namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.ProbationManagement;
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
        #region Profile Page - Association

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
                        CTSCustID = paras.CTSCustId,
                        AssociationStatus = paras.AssociationStatus,
                        AssociationType = paras.AssociationType,
                        AccountStatuses = paras.AccountStatusIds.Split(',').Select(int.Parse),
                        SubscriberIDs = paras.SubscriberIDs.Split(',').Select(int.Parse),
                        CategoryIDs = paras.CategoryIDs.Split(',').Select(int.Parse),
                        Skip = paras.Skip,
                        Pagesize = paras.Take
                    })?.Data.FirstOrDefault();

                    return result ?? new GraphAssociationPaging() { AssociationList = new List<GraphAssociationModel>(), TotalItems = 0 };
                }
            }
            catch (Exception ex)
            {
                LogService.Error("WEB - GetDirectAssociationProfile failed: " + ex.Message + ex.StackTrace + "\n"
                       + ex.InnerException.Message + ex.InnerException.Source + ((Neo4j.Driver.Neo4jException)ex.InnerException).Code
                       + ex.InnerException.StackTrace);
            }

            return new GraphAssociationPaging() { AssociationList = new List<GraphAssociationModel>(), TotalItems = 0 }; ;
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

        #endregion Profile Page - Association

        #region Flagged Evidence Level API

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
        /// Get association report
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetAssociationReport")]
        [HttpPost]
        public GraphAssociationReportPaging GetAssociationReport([FromBody] AssociationReportParameterModel paras)
        {
            var result = new GraphAssociationReportPaging() { AssociationList = null, TotalItems = 0 };

            try
            {
                if (paras != null)
                {
                    string scriptName = string.Empty;
                    switch (paras.AssociationType)
                    {
                        case 1:
                            {
                                scriptName = "GetAssociationReportByDevice";
                                break;
                            }
                        case 2:
                            {
                                scriptName = "GetAssociationReportByIP";
                                break;
                            }
                        case 3:
                            {
                                scriptName = "GetAssociationReportByManual";
                                break;
                            }
                    }

                    if (string.IsNullOrEmpty(scriptName))
                    {
                        return result;
                    }

                    var paramForGraph = new
                    {
                        ip_AssCreatedFrom = paras.AssociatedDateFrom.AddHours(4),
                        ip_AssCreatedTo = paras.AssociatedDateTo.AddHours(4),
                        ip_SubscriberList = paras.SubscriberIDs.Split(',').Select(int.Parse),
                        ip_AccountStatus = paras.AccountStatuses.Split(',').Select(int.Parse),
                        ip_EvidenceID = paras.EvidenceIDs?.Trim().Split(',').Select(int.Parse),
                        ip_AssStatus = paras.AssociationStatus,
                        ip_TakeRows = paras.TakeRow,
                        ip_SkipRows = paras.SkipRow
                    };

                    result.AssociationList = DbSessionFactory.Execute<AssociationReportModel>(scriptName, paramForGraph).Data;
                    result.TotalItems = DbSessionFactory.Execute<int>("GetAssociationReportByDevice_CountTotal", paramForGraph).Data.FirstOrDefault();

                    LogService.Info("result.TotalItems: " + result.TotalItems.ToString());
                    LogService.Info("result.AssociationList: " + result.AssociationList.Count().ToString());

                    return result;
                }
            }
            catch (Exception ex)
            {
                LogService.Error("GetAssociationReport failed: " + ex.Message + ex.StackTrace + "\n"
                        + ex.InnerException.Message + ex.InnerException.Source + ((Neo4j.Driver.Neo4jException)ex.InnerException).Code
                        + ex.InnerException.StackTrace);
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

        #endregion API (Need to remove without in use)
    }
}