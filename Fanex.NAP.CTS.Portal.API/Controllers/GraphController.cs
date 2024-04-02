namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.CustomerInfo;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.GraphDB.AssDetection;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using Fanex.NAP.CTS.Portal.API.ActionFilter;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociationDetection;
    using Fanex.NAP.CTS.ViewModel.AssociationReport;
    using Fanex.NAP.CTS.ViewModel.ProbationManagement;
    using Fanex.NAP.SPU.Graph.Core;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    /// <summary>
    /// Graph Controller - Using for CTS site
    /// </summary>
    public class GraphController : ApiController
    {
        private readonly IAssociatedAccountService associationService;
        private readonly IAssociationDetectionService associationDetectionService;
        private readonly IAssociationReportService associationReportService;

        /// <summary>
        /// Constructor
        /// </summary>
        public GraphController()
        {
            associationService = new AssociatedAccountService();
            associationDetectionService = new AssociationDetectionService();
            associationReportService = new AssociationReportService();
        }   

        #region API - Flagged Evidence Level

        /// <summary>
        /// API Get GetFlaggedEvidenceForLevel01 For Taiwan
        /// </summary>
        /// <param name="custIdList"></param>
        /// <returns></returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetFlaggedEvidenceForLevel01API)]
        public JsonResult<List<CustomerEvidenceForTW>> GetFlaggedEvidenceForLevel01(string custIdList)
        {
            var result = new List<CustomerEvidenceForTW>();

            try
            {
                if (!string.IsNullOrEmpty(custIdList))
                {
                    if (ConfigSettings.GetFlaggedEviByAssFromMySQL)
                    {
                        result = associationService.GetFlaggedEvidenceByDirectAssForAPI(custIdList)?.ToList();
                    }
                    else
                    {
                        var custIds = custIdList.Split(',').Select(a => Convert.ToInt32(a));

                        Parallel.Invoke(() =>
                        {
                            var ceLv0 = DbSessionFactory.Execute<CustomerEvidenceForTW>("GetCustomerEvidenceLevel0", new { CustID = custIds }).Data;
                            result.AddRange(ceLv0);
                        },
                        () =>
                        {
                            var ceLv1 = DbSessionFactory.Execute<CustomerEvidenceForTW>("GetCustomerEvidenceLevel1", new { CustID = custIds }).Data;
                            result.AddRange(ceLv1);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"WEB - GetFlaggedEvidenceForLevel01 - Data input: {custIdList} - failed: " + ex.Message + ex.StackTrace);
            }

            return Json(result ?? new List<CustomerEvidenceForTW>());
        }

        /// <summary>
        /// API Get GetFlaggedEvidenceForLevel2 For Taiwan
        /// </summary>
        /// <param name="custIdList"></param>
        /// <returns></returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetFlaggedEvidenceForLevel2API)]
        public JsonResult<List<CustomerEvidenceForTW>> GetFlaggedEvidenceForLevel2(string custIdList)
        {
            var result = new List<CustomerEvidenceForTW>();

            try
            {
                if (!string.IsNullOrEmpty(custIdList))
                {
                    if (ConfigSettings.GetFlaggedEviByAssFromMySQL)
                    {
                        result = associationService.GetFlaggedEvidenceByLevel2AssForAPI(custIdList)?.ToList();
                    }
                    else
                    {
                        var custIds = custIdList.Split(',').Select(a => Convert.ToInt32(a));

                        result = DbSessionFactory.Execute<CustomerEvidenceForTW>("GetCustomerEvidenceLevel2", new { CustID = custIds }).Data.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"WEB - GetFlaggedEvidenceForLevel2 - Data input: {custIdList} - failed: " + ex.Message + ex.StackTrace);
            }

            return Json(result ?? new List<CustomerEvidenceForTW>());
        }

        #endregion API - Flagged Evidence Level

        #region WEB - Association Report

        /// <summary>
        /// Get association report
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetAssociationReport")]
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetAssociationReportAPI)]
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
                                return associationReportService.MergeAssociationReport(custInfo, custAssociation, paras);
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

        #endregion WEB - Association Report

        #region WEB - Association Detection Report

        /// <summary>
        /// Get association report
        /// </summary>
        /// <param name="ctsCustIDs"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetAssociationDetectionReport")]
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetAssociationDetectionReportAPI)]
        public IEnumerable<GetAssociationDetectionModel> GetAssociationDetectionReport([FromBody] string ctsCustIDs)
        {
            try
            {
                if (!string.IsNullOrEmpty(ctsCustIDs))
                {
                    var ip_CTSCustIDs = ctsCustIDs.Split(',').Select(long.Parse);

                    var custAssociations = DbSessionFactory.Execute<AssociationLevelGraphDbViewModel>("GetAssociationDetectionReport", new
                    {
                        ip_CTSCustIDs = ip_CTSCustIDs,
                        ip_ExcludeSubscriberIDs = new List<int>() { 4521, 1270, 4408 }
                    }).Data;

                    if (custAssociations != null && custAssociations.Any())
                    {
                        var groupAss = associationDetectionService.ClassifyGroupAssociation(custAssociations, ip_CTSCustIDs);

                        return associationDetectionService.GetCustInfoAssDetectionReport(groupAss);
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

        /// <summary>
        /// DetectAssociationByPrimaryOrAgentType
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("DetectAssociationByPrimaryOrAgentType")]
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetAssociationDetectionReportAPI)]
        public IEnumerable<DetectAssociationByPrimaryModel> DetectAssociationByPrimaryOrAgentType([FromBody] DetectAssByPrimaryOrAgentParams paras)
        {
            try
            {
                if (!string.IsNullOrEmpty(paras.CTSCustIDs))
                {
                    var ip_CTSCustIDs = paras.CTSCustIDs.Split(',').Select(long.Parse);

                    DbSessionFactory.Execute<object>("AssDetection_DropInMemoryGraph");
                    var scriptName = paras.AssType == 1 ? "AssDetection_CreateInMemoryGraph" : "AssDetection_CreateInMemoryGraph_Agent";
                    DbSessionFactory.Execute<object>(scriptName, new { CTSCustIDs = ip_CTSCustIDs });
                    var isGraphExisting = DbSessionFactory.Execute<bool>("AssDetection_CheckIfGraphExists").Data.FirstOrDefault();
                    if (isGraphExisting)
                    {
                        DbSessionFactory.Execute<object>("AssDetection_RunWCCAlgo");
                    
                        var custAssociations = DbSessionFactory.Execute<DetectAssociationByPrimaryModel>("AssDetection_GetResult", new
                        {
                            CTSCustIDs = ip_CTSCustIDs,
                            AssType = paras.AssType
                        }).Data;

                        return custAssociations;
                    }
                }
            }
            catch (Exception ex)
            {
                string mess = $"WEB - {ConfigSettings.EnvironmentName} - DetectAssociationByPrimaryType failed: " + ex.Message + ex.StackTrace + "\n";
                if (ex.InnerException != null)
                {
                    mess += ex.InnerException.Message + ex.InnerException.Source + ((Neo4j.Driver.Neo4jException)ex.InnerException).Code
                        + ex.InnerException.StackTrace;
                }
                
                LogService.Error(mess);
            }

            return new List<DetectAssociationByPrimaryModel>();
        }

        /// <summary>
        /// DetectAssociationGetDetail
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("DetectAssociationGetDetail")]
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetAssociationDetectionReportAPI)]
        public IEnumerable<GetAssociationDetectDetailModel> DetectAssociationGetDetail([FromBody] DetectAssGetDetailParams paras)
        {
            var custAssociations = new List<GetAssociationDetectDetailModel>();
            try
            {
                if (paras.RootCTSCustID > 0)
                {
                    var ip_assCTSCustIDs = paras.AssCTSCustIDs.Split(',').Select(long.Parse);

                    if (paras.GetPrimaryType)
                    {
                        var custAssociations_Primary = DbSessionFactory.Execute<GetAssociationDetectDetailModel>("AssDetection_GetDetail_PrimaryType", new
                        {
                            RootCTSCustID = paras.RootCTSCustID,
                            AssCTSCustIDs = ip_assCTSCustIDs
                        }).Data;

                        custAssociations.AddRange(custAssociations_Primary);
                    }

                    if (paras.GetAgentType)
                    {
                        var custAssociations_Agent = DbSessionFactory.Execute<GetAssociationDetectDetailModel>("AssDetection_GetDetail_AgentType", new
                        {
                            RootCTSCustID = paras.RootCTSCustID,
                            AssCTSCustIDs = ip_assCTSCustIDs
                        }).Data;

                        custAssociations.AddRange(custAssociations_Agent);
                    }
                }
            }
            catch (Exception ex)
            {
                string mess = $"WEB - {ConfigSettings.EnvironmentName} - DetectAssociationGetDetail failed: " + ex.Message + ex.StackTrace + "\n";
                if (ex.InnerException != null)
                {
                    mess += ex.InnerException.Message + ex.InnerException.Source + ((Neo4j.Driver.Neo4jException)ex.InnerException).Code
                        + ex.InnerException.StackTrace;
                }

                LogService.Error(mess);
            }

            return custAssociations;
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetStatisticAssociationAPI)]
        public CustomerInfoAssociationAccountModel GetStatisticAssociation(long ctsCustId)
        {
            try
            {
                var queryResult = DbSessionFactory.Execute<CustomerInfoAssociationAccountModel>("GetStatisticForCustomer", new { CTSCustID = ctsCustId });

                var result = queryResult.Data.FirstOrDefault();

                if (result == null)
                {
                    return new CustomerInfoAssociationAccountModel();
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetDirectAssociationAPI)]
        public AssociatedAccountPagingModel GetDirectAssociation([FromBody] DirectAssociationParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<AssociatedAccountPagingModel>("GetAssociationOneLevelByCustId", new
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
                    return new AssociatedAccountPagingModel() { AssociationList = null, TotalItems = 0 };
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetDirectAssociationByManualAPI)]
        public AssociatedAccountPagingModel GetDirectAssociationByManual([FromBody] DirectAssociationParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<AssociatedAccountPagingModel>("GetAssociationOneLevelByCustIdByManual", new
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

                return result ?? new AssociatedAccountPagingModel() { AssociationList = null, TotalItems = 0 };
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationByManual failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationByDevice")]
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetDirectAssociationByDeviceAPI)]
        public AssociatedAccountPagingModel GetDirectAssociationByDevice([FromBody] DirectAssociationParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<AssociatedAccountPagingModel>("GetAssociationOneLevelByCustIdByDevice", new
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
                    return new AssociatedAccountPagingModel() { AssociationList = null, TotalItems = 0 };
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetDirectAssociationByIPAPI)]
        public AssociatedAccountPagingModel GetDirectAssociationByIP([FromBody] DirectAssociationParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<AssociatedAccountPagingModel>("GetAssociationOneLevelByCustIdByIP", new
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
                    return new AssociatedAccountPagingModel() { AssociationList = null, TotalItems = 0 };
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetDirectAssociationByAIAPI)]
        public AssociatedAccountPagingModel GetDirectAssociationByAI([FromBody] DirectAssociationParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<AssociatedAccountPagingModel>("GetAssociationOneLevelByCustIdByAI", new
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
                    return new AssociatedAccountPagingModel() { AssociationList = null, TotalItems = 0 };
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetDirectAssociationMultiCustAPI)]
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetInDirectAssociationAPI)]
        public IEnumerable<AssociatedAccountModel> GetInDirectAssociation([FromBody] int ctsCustId)
        {
            try
            {
                var result = DbSessionFactory.Execute<AssociatedAccountModel>("GetAssociationNLevelByCustId", new { CTSCustID = ctsCustId }).Data;
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetShortestPathAPI)]
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GroupingCustomerAPI)]
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetDirectAssociationDevicesAPI)]
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
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetDetailAssociationReportAPI)]
        public AssociatedAccountPagingModel GetDetailAssociationReport([FromBody] AssociationDetailReportParameterModel paras)
        {
            var result = new AssociatedAccountPagingModel() { AssociationList = null, TotalItems = 0 };

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
                        var queryResult = DbSessionFactory.Execute<AssociatedAccountPagingModel>(script, new
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

        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.ScanCTSCustomerAPI)]
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