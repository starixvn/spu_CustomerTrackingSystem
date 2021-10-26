namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using Fanex.NAP.CTS.ViewModel.AssociationDetection;
    using Fanex.NAP.CTS.ViewModel.AssociationReport;
    using Fanex.NAP.CTS.ViewModel.ProbationManagement;
    using Fanex.NAP.SPU.Graph.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;


    /// <summary>
    /// Graph Controller - Using for CTS site
    /// </summary>
    public class GraphController : ApiController
    {
        #region WEB - Association Profile Page

        /// <summary>
        /// Get Direct All Association for WEB Profile Page
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetDirectAssociationProfile")]
        [HttpPost]
        public GraphAssociationPaging GetDirectAssociationProfile([FromBody] AssociationProfileParameterModel paras)
        {
            try
            {
                if (!string.IsNullOrEmpty(paras.SubscriberIDs) || !string.IsNullOrEmpty(paras.CategoryIDs))
                {
                    string queryScript = GetQueryScriptForAssProfile(paras.AssociationType);

                    var result = DbSessionFactory.Execute<GraphAssociationPaging>(queryScript, new
                    {
                        ip_CTSCustID = paras.CTSCustId,
                        ip_AssociationStatus = paras.AssociationStatus,
                        ip_AssociationType = paras.AssociationType,
                        ip_AccountStatuses = paras.AccountStatusIds.Split(',').Select(int.Parse),
                        ip_SubscriberIDs = paras.SubscriberIDs.Split(',').Select(int.Parse),
                        ip_CategoryIDs = paras.CategoryIDs.Split(',').Select(int.Parse),
                        ip_Skip = paras.Skip,
                        ip_PageSize = paras.Take
                    })?.Data.FirstOrDefault();

                    return result ?? new GraphAssociationPaging() { AssociationList = new List<GraphAssociationModel>(), TotalItems = 0 };
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"WEB - {ConfigSettings.EnvironmentName} - Get Association Tab Profile failed: " + ex.Message + ex.StackTrace + "\n"
                       + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                       + ex.InnerException.StackTrace);
            }

            return new GraphAssociationPaging() { AssociationList = new List<GraphAssociationModel>(), TotalItems = 0 };
        }

        private string GetQueryScriptForAssProfile(int associationType)
        {
            switch (associationType)
            {
                case 1:
                    return "GetDirectAssociationProfileByDevice";

                case 2:
                    return "GetDirectAssociationProfileByAI";

                case 3:
                    return "GetDirectAssociationProfileByManual";

                case 4:
                    return "GetDirectAssociationProfileByIP";

                default:
                    return "GetDirectAssociationProfile";
            }
        }

        #endregion WEB - Association Profile Page

        #region API - Flagged Evidence Level

        [HttpPost]
        public JsonResult<List<CustomerEvidenceForTW>> GetFlaggedEvidenceForLevel01(string custIdList)
        {
            var result = new List<CustomerEvidenceForTW>();
            try
            {
                var custIds = custIdList.Split(',').Select(a => Convert.ToInt32(a));
                Parallel.Invoke(()=> 
                {
                    var ceLv0 = DbSessionFactory.Execute<CustomerEvidenceForTW>("GetCustomerEvidenceLevel0", new { CustID = custIds }).Data;
                    result.AddRange(ceLv0);
                },
                ()=> 
                {
                    var ceLv1 = DbSessionFactory.Execute<CustomerEvidenceForTW>("GetCustomerEvidenceLevel1", new { CustID = custIds }).Data;
                    result.AddRange(ceLv1);
                });
                return Json(result);
            }
            catch (Exception ex)
            {
                LogService.Error("WEB - GetFlaggedEvidenceForLevel01 failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }
        
        [HttpPost]
        public JsonResult<List<CustomerEvidenceForTW>> GetFlaggedEvidenceForLevel2(string custIdList)
        {
            try
            {
                var custIds = custIdList.Split(',').Select(a => Convert.ToInt32(a));
                var result = DbSessionFactory.Execute<CustomerEvidenceForTW>("GetCustomerEvidenceLevel2", new { CustID = custIds }).Data.ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                LogService.Error("WEB - GetFlaggedEvidenceForLevel2 failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        #endregion

        #region WEB - Association Report

        /// <summary>
        /// Get association report
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetAssociationReport")]
        [HttpPost]
        public AssociationReportViewModel GetAssociationReport([FromBody] AssociationReportParameterModel paras)
        {
            try
            {
                if (paras != null && paras.SubscriberIDs.Any() && paras.EvidenceIDs.Any() && paras.AssociationStatus != -1)
                {
                    string queryScript = GetQueryScriptForAssReport(paras.AssociationType);

                    if (!string.IsNullOrEmpty(queryScript))
                    {
                        var paraForCustInfo = new
                        {
                            AccountStatusIDs = paras.AccountStatusIDs,
                            SubscriberIDs = paras.SubscriberIDs,
                            EvidenceIDs = paras.EvidenceIDs,
                        };

                        var custInfo = DbSessionFactory.Execute<AssociationReportViewModel>("GetAssReportCustInfo", paraForCustInfo).Data.FirstOrDefault();

                        if (custInfo != null && custInfo.AssociationList != null && custInfo.AssociationList.Any())
                        {
                            List<int> ctsCustIds = custInfo.AssociationList.Select(c => c.CTSCustId).Distinct().ToList();

                            var paramater = new
                            {
                                CTSCustIDs = ctsCustIds,
                                FromAssDate = paras.FromAssDate,
                                ToAssDate = paras.ToAssDate,
                                AssociationStatus = paras.AssociationStatus,
                            };

                            var custAssociation = DbSessionFactory.Execute<AssociationReportViewModel>(queryScript, paramater).Data.FirstOrDefault();

                            if (custAssociation != null && custAssociation.TotalItems > 0)
                            {
                                return MergeAssociationReport(custInfo, custAssociation, paras);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error("WEB - GetAssociationReport failed: " + ex.Message + ex.StackTrace + "\n"
                        + ex.InnerException.Message + ex.InnerException.Source + ((Neo4j.Driver.Neo4jException)ex.InnerException).Code
                        + ex.InnerException.StackTrace);
            }

            return new AssociationReportViewModel() { AssociationList = new List<AssociationViewModel>(), TotalItems = 0 };
        }

        private string GetQueryScriptForAssReport(int associationType)
        {
            switch (associationType)
            {
                case 1:
                    {
                        return "GetAssReportByDevice";
                    }
                case 3:
                    {
                        return "GetAssReportByManual";
                    }
                default:
                    return string.Empty;
            }
        }

        private AssociationReportViewModel MergeAssociationReport(AssociationReportViewModel custInfo, AssociationReportViewModel custAss, AssociationReportParameterModel para)
        {
            var result = new List<AssociationViewModel>();

            if (custInfo.AssociationList.Any() && custAss.AssociationList.Any())
            {
                result = custAss.AssociationList.GroupJoin(custInfo.AssociationList,
                                a => a.CTSCustId,
                                i => i.CTSCustId,
                                (a, i) => new { a, i })
                                .SelectMany(
                                    group => group.i.DefaultIfEmpty(),
                                    (group, temp) => new AssociationViewModel
                                    {
                                        CTSCustId = group.a.CTSCustId,
                                        UserName = (temp != null) ? temp.UserName : string.Empty,
                                        AccountStatus = (temp != null) ? temp.AccountStatus : string.Empty,
                                        SubscriberId = (temp != null) ? temp.SubscriberId : 0,
                                        SubscriberName = (temp != null) ? temp.SubscriberName : string.Empty,
                                        EvidenceCodes = (temp != null) ? temp.EvidenceCodes : string.Empty,
                                        CategoryName = group.a.CategoryName
                                    }).OrderBy(c => c.SubscriberName).Skip(para.SkipRow).Take(para.TakeRow).ToList();

            }

            return new AssociationReportViewModel()
            {
                AssociationList = result ?? new List<AssociationViewModel>(),
                TotalItems = custAss.TotalItems
            };
        }

        #endregion WEB - Association Report

        #region WEB - Association Detection Report

        /// <summary>
        /// Get association report
        /// </summary>
        /// <param name="ctsCustIDs"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetAssociationDetectionReport")]
        [HttpPost]
        public IEnumerable<GetAssociationDetectionModel> GetAssociationDetectionReport([FromBody] string ctsCustIDs)
        {
            try
            {
                if (!string.IsNullOrEmpty(ctsCustIDs))
                {
                    var ip_CTSCustIDs = ctsCustIDs.Split(',').Select(int.Parse);

                    var custAssociations = DbSessionFactory.Execute<AssociationLevelGraphDbViewModel>("GetAssociationDetectionReport", new
                    {
                        ip_CTSCustIDs = ip_CTSCustIDs,
                        ip_ExcludeSubscriberIDs = new List<int>() { 4521, 1270, 4408 }
                    }).Data;

                    if (custAssociations != null && custAssociations.Any())
                    {
                        var assDetection = new List<GetAssociationDetectionModel>();

                        custAssociations.ToList().ForEach(i =>
                        {
                            if (i.Associations != null && i.Associations.Any())
                            {
                                assDetection = ClassifyGroupAssociation(i, assDetection);
                            }
                        });

                        return GetCustInfoAssDetectionReport(assDetection);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"WEB - {ConfigSettings.EnvironmentName} - GetAssociationDetectionReport failed: " + ex.Message + ex.StackTrace + "\n"
                        + ex.InnerException.Message + ex.InnerException.Source + ((Neo4j.Driver.Neo4jException)ex.InnerException).Code
                        + ex.InnerException.StackTrace);
            }

            return new List<GetAssociationDetectionModel>();
        }

        private List<GetAssociationDetectionModel> ClassifyGroupAssociation(AssociationLevelGraphDbViewModel association, List<GetAssociationDetectionModel> assDetection)
        {
            var temp = assDetection;

            int maxGroupId = temp.Any() ? temp.Max(c => c.GroupId) : 0;

            if (!temp.Any(x => x.CTSCustId.Equals(association.CTSCustIdLv0)))
            {
                temp.Add(new GetAssociationDetectionModel { GroupId = maxGroupId + 1, CTSCustId = association.CTSCustIdLv0 });
            }

            if (association.Associations.Any())
            {
                int rootGroupId = temp.SingleOrDefault(c => c.CTSCustId == association.CTSCustIdLv0).GroupId;

                association.Associations.ForEach(eachAss => {
                    if (!temp.Any(x => x.CTSCustId.Equals(eachAss)))
                    {
                        temp.Add(new GetAssociationDetectionModel { GroupId = rootGroupId, CTSCustId = eachAss });
                    }
                });
            }

            return temp;
        }

        private IEnumerable<GetAssociationDetectionModel> GetCustInformation(List<GetAssociationDetectionModel> assDetection)
        {
            if (assDetection != null && assDetection.Any())
            {
                var ip_CTSCustIDs = assDetection.Select(c => c.CTSCustId);

                var result = DbSessionFactory.Execute<GetAssociationDetectionModel>("GetCustInformation", new { ip_CTSCustIDs = ip_CTSCustIDs }).Data;

                return result ?? new List<GetAssociationDetectionModel>();
            }

            return new List<GetAssociationDetectionModel>();
        }

        private IEnumerable<GetAssociationDetectionModel> GetCustInfoAssDetectionReport(List<GetAssociationDetectionModel> assDetection)
        {
            if (assDetection != null && assDetection.Any())
            {
                var custInfo = GetCustInformation(assDetection);

                if (custInfo.Any())
                {
                    var result = custInfo.Join(
                                    assDetection,
                                    x => x.CTSCustId,
                                    y => y.CTSCustId,
                                    (x, y) => new GetAssociationDetectionModel
                                    {
                                        CTSCustId = x.CTSCustId,
                                        GroupId = y.GroupId,
                                        CategoryIds = x.CategoryIds,
                                        CustId = x.CustId,
                                        CustomerCategory = string.IsNullOrEmpty(x.CustomerCategory) ? "-" : x.CustomerCategory,
                                        CustStatus = x.CustStatus,
                                        CustSubId = x.CustSubId,
                                        Evidence = string.IsNullOrEmpty(x.Evidence) ? "-" : x.Evidence,
                                        ParentIds = x.ParentIds,
                                        UserName = x.UserName
                                    });

                    return result ?? new List<GetAssociationDetectionModel>();
                }
            }

            return new List<GetAssociationDetectionModel>();
        }

        #endregion WEB - Association Detection Report

        #region API (Need to remove without in use)

        /// <summary>
        /// Get association statistic
        /// </summary>
        /// <param name="ctsCustId"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetStatisticAssociation")]
        [HttpGet]
        public CustomerAssociationByCTSCustIdModel GetStatisticAssociation(int ctsCustId)
        {
            try
            {
                var queryResult = DbSessionFactory.Execute<CustomerAssociationByCTSCustIdModel>("GetStatisticForCustomer", new { CTSCustID = ctsCustId });

                var result = queryResult.Data.ToList().FirstOrDefault();

                if (result == null)
                {
                    return new CustomerAssociationByCTSCustIdModel();
                }

                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetStatisticAssociation failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Get all association one level
        /// </summary>
        /// <param name="ctsCustId"></param>
        /// <param name="assStatus"></param>
        /// <param name="takeRow"></param>
        /// <param name="skipRow"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetDirectAssociation")]
        [HttpPost]
        public GraphAssociationPaging GetDirectAssociation([FromBody] DirectAssociationParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelByCustId", new
                {
                    CTSCustID = paras.ctsCustId,
                    AssStatus = paras.assStatus,
                    AccountStatuses = paras.accountStatuses.Split(',').Select(int.Parse),
                    SiteIDs = paras.subIDs.Split(',').Select(int.Parse),
                    Skip = paras.skipRow,
                    Pagesize = paras.takeRow
                }).Data.FirstOrDefault();

                if (result == null)
                {
                    return new GraphAssociationPaging() { AssociationList = null, TotalItems = 0 };
                }

                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociation failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationByManual")]
        [HttpPost]
        public GraphAssociationPaging GetDirectAssociationByManual([FromBody] DirectAssociationParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelByCustIdByManual", new
                {
                    CTSCustID = paras.ctsCustId,
                    AssStatus = paras.assStatus,
                    IsAssReport = 0,
                    AssCreatedFrom = DateTime.MinValue,
                    AssCreatedTo = DateTime.MinValue,
                    AccountStatuses = paras.accountStatuses.Split(',').Select(int.Parse),
                    SiteIDs = paras.subIDs.Split(',').Select(int.Parse),
                    Skip = paras.skipRow,
                    Pagesize = paras.takeRow
                }).Data.FirstOrDefault();

                return result ?? new GraphAssociationPaging() { AssociationList = null, TotalItems = 0 };
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationByManual failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationByDevice")]
        [HttpPost]
        public GraphAssociationPaging GetDirectAssociationByDevice([FromBody] DirectAssociationParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelByCustIdByDevice", new
                {
                    CTSCustID = paras.ctsCustId,
                    AssStatus = paras.assStatus,
                    IsAssReport = 0,
                    AssCreatedFrom = DateTime.MinValue,
                    AssCreatedTo = DateTime.MinValue,
                    AccountStatuses = paras.accountStatuses.Split(',').Select(int.Parse),
                    SubscriberIDs = paras.subIDs.Split(',').Select(int.Parse),
                    Skip = paras.skipRow,
                    Pagesize = paras.takeRow
                }).Data.FirstOrDefault();

                if (result == null)
                {
                    return new GraphAssociationPaging() { AssociationList = null, TotalItems = 0 };
                }

                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationByDevice failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationByIP")]
        [HttpPost]
        public GraphAssociationPaging GetDirectAssociationByIP([FromBody] DirectAssociationParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelByCustIdByIP", new
                {
                    CTSCustID = paras.ctsCustId,
                    AssStatus = paras.assStatus,
                    IsAssReport = 0,
                    AssCreatedFrom = DateTime.MinValue,
                    AssCreatedTo = DateTime.MinValue,
                    AccountStatuses = paras.accountStatuses.Split(',').Select(int.Parse),
                    SiteIDs = paras.subIDs.Split(',').Select(int.Parse),
                    Skip = paras.skipRow,
                    Pagesize = paras.takeRow
                }).Data.FirstOrDefault();

                if (result == null)
                {
                    return new GraphAssociationPaging() { AssociationList = null, TotalItems = 0 };
                }

                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociation failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationByAI")]
        [HttpPost]
        public GraphAssociationPaging GetDirectAssociationByAI([FromBody] DirectAssociationParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelByCustIdByAI", new
                {
                    CTSCustID = paras.ctsCustId,
                    AssStatus = paras.assStatus,
                    IsAssReport = 0,
                    AssCreatedFrom = DateTime.MinValue,
                    AssCreatedTo = DateTime.MinValue,
                    AccountStatuses = paras.accountStatuses.Split(',').Select(int.Parse),
                    SiteIDs = paras.subIDs.Split(',').Select(int.Parse),
                    Skip = paras.skipRow,
                    Pagesize = paras.takeRow
                }).Data.FirstOrDefault();

                if (result == null)
                {
                    return new GraphAssociationPaging() { AssociationList = null, TotalItems = 0 };
                }

                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationByAI failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationMultiCust")]
        [HttpPost]
        public IEnumerable<GraphDirectAssociationModel> GetDirectAssociationMultiCust([FromBody] List<long> ctsCustIds)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphDirectAssociationModel>("GetAssociationOneLevelByCustIds", new { CTSCustIDs = ctsCustIds }).Data;
                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationMultiCust failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Get Association with N Level
        /// </summary>
        /// <param name="ctsCustId"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetInDirectAssociation")]
        [HttpPost]
        public IEnumerable<GraphAssociationModel> GetInDirectAssociation([FromBody] int ctsCustId)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationNLevelByCustId", new { CTSCustID = ctsCustId }).Data;
                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetInDirectAssociation failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Get shortest path between 2 customers
        /// </summary>
        /// <param name="ctsCustIdFrom"></param>
        /// <param name="ctsCustIdTo"></param>
        /// <returns>Json string</returns>
        [System.Web.Http.AcceptVerbs("GetShortestPath")]
        [HttpGet]
        public JsonResult<ShortestPathOfAccount> GetShortestPath(int ctsCustIdFrom, int ctsCustIdTo)
        {
            var result = DbSessionFactory.ExecuteSingle<ShortestPathOfAccount>("GetShortestPath", new { StartCustId = ctsCustIdFrom, EndCustId = ctsCustIdTo }).Data;
            return Json(result);
        }

        /// <summary>
        /// Grouping customer
        /// </summary>
        /// <param name="custIds">List custId</param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<CustomerGroup> GroupingCustomer([FromBody] IEnumerable<int> custIds)
        {
            IEnumerable<CustomerGroup> result = null;
            try
            {
                if (custIds.Any())
                {
                    result = DbSessionFactory.Execute<CustomerGroup>("GroupCustomerByAssociationOneLevel", new { CustIds = custIds.Distinct() }).Data;
                }
            }
            catch (Exception ex)
            {
                LogService.Error("GroupingCustomer failed: " + ex.Message + ex.StackTrace);
            }

            return result;
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationDevices")]
        [HttpGet]
        public DirectAssociationDeviceModel GetDirectAssociationDevices(long custId)
        {
            var result = new DirectAssociationDeviceModel();

            try
            {
                result = DbSessionFactory.Execute<DirectAssociationDeviceModel>("GetDevicesIdByCustId", new { ip_CustID = custId }).Data.SingleOrDefault();
                result.DirectAssociation = DbSessionFactory.Execute<DirectAssociationModel>("GetAssociatedDevicesByCustId", new { ip_CustID = custId }).Data;
                result.TotalDirectAssociation = result.DirectAssociation.Count();

                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationDevices failed: " + ex.Message + ex.StackTrace);
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetDetailAssociationReport")]
        [HttpPost]
        public GraphAssociationPaging GetDetailAssociationReport([FromBody] AssociationDetailReportParameterModel paras)
        {
            var result = new GraphAssociationPaging() { AssociationList = null, TotalItems = 0 };

            try
            {
                if (paras != null)
                {
                    string script = string.Empty;
                    switch (paras.AssociationType)
                    {
                        case 1:
                            {
                                script = "GetAssociationOneLevelByCustIdByDevice";
                                break;
                            }
                        case 2:
                            {
                                script = "GetAssociationOneLevelByCustIdByIP";
                                break;
                            }
                        case 3:
                            {
                                script = "GetAssociationOneLevelByCustIdByManual";
                                break;
                            }
                    }

                    if (!string.IsNullOrEmpty(script))
                    {
                        var queryResult = DbSessionFactory.Execute<GraphAssociationPaging>(script, new
                        {
                            CTSCustID = paras.CTSCustId,
                            AssStatus = paras.AssociationStatus,
                            IsAssReport = 1,
                            AssCreatedFrom = paras.AssociatedDateFrom,
                            AssCreatedTo = paras.AssociatedDateTo,
                            AccountStatuses = new List<int>(),
                            SubscriberIDs = new List<int>(),
                            Skip = paras.SkipRow,
                            Pagesize = paras.TakeRow
                        }).Data.FirstOrDefault();

                        return queryResult;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error("GetDetailAssociationReport failed: " + ex.Message + ex.StackTrace + "\n"
                        + ex.InnerException.Message + ex.InnerException.Source + ((Neo4j.Driver.Neo4jException)ex.InnerException).Code
                        + ex.InnerException.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// Get association report
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetProbationReport")]
        [HttpPost]
        public IEnumerable<AssociationWithPAModel> GetProbationReport([FromBody] ProbationParameterModel paras)
        {
            IEnumerable<AssociationWithPAModel> result = null;
            try
            {
                result = DbSessionFactory.Execute<AssociationWithPAModel>("GetProbationWithAssociatedPA",
                    new
                    {
                        ip_CustIDs = paras.CustIDs,
                        ip_isAssociatedWithPA = paras.IsAssociatedWithPA
                    }).Data;

                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetAssociationReport failed: " + ex.Message + ex.StackTrace);
            }

            return result;
        }

        [HttpGet]
        public void ScanCTSCustomer()
        {
            try
            {
                 DbSessionFactory.Execute("ScanCTSCustomer");

            }
            catch (Exception ex)
            {
                LogService.Error("ScanCTSCustomer failed: " + ex.Message + ex.StackTrace);
            }

        }

        #endregion API (Need to remove without in use)
    }
}