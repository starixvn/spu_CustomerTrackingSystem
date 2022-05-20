namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.TWProblemAccountDetection;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.API.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Model.API.UpdateCategoryWhenPAUpdated;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.Model.LogInputData;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor.TWProblemAccount;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Portal.API.ActionFilter;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Implement.Api;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using Fanex.NAP.CTS.ViewModel.API.ProblemCaseMonitor;
    using Fanex.NAP.CTS.ViewModel.SpecialCustomerClass;
    using Fanex.NAP.SPU.Graph.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using IProblemCaseMonitorService = Service.Interface.Api.IProblemCaseMonitorService;

    /// <summary>
    /// CtsController
    /// </summary>
    public class CtsController : ApiController
    {
        private readonly IAccountService _accountService;
        private readonly IEvidenceService _evidenceService;
        private readonly ICustomerEvidenceService _custEvidenceService;
        private readonly IAssociatedAccountService _associationService;
        private readonly IProblemAccountMonitorService _problemAccountMonitorService;
        private readonly ICategoryService _categoryService;
        private readonly IRescanCustomerClassificationService _rescanClassificationService;
        private readonly ICustomerClassificationService _custClassificationService;
        private readonly ISubscriberService _subscriberService;
        private readonly IProblemCaseMonitorService _problemCaseMonitorService;
        private readonly IProblemAccountManagementService _problemAccountManagementService;        
        private readonly ILogInputDataService logInputDataService;
        private readonly IUpdateCategoryService _updateCategoryService;
        private readonly IAntiFraudCenterService _antiFraudCenterService;
        private readonly ISpecialCustomerClassService _customerClassService;
        private readonly IProblemAccountClassificationService _paClassificationService;

        /// <summary>
        /// Using for external site (TW, ExtAcc site...)
        /// </summary>
        public CtsController()
        {
            _accountService = new AccountService();
            _evidenceService = new EvidenceService();
            _associationService = new AssociatedAccountService();
            _custEvidenceService = new CustomerEvidenceService();
            _problemAccountMonitorService = new ProblemAccountMonitorService();
            _categoryService = new CategoryService();
            _rescanClassificationService = new RescanCustomerClassificationService();
            _custClassificationService = new CustomerClassificationService();
            _subscriberService = new SubscriberService();
            _problemCaseMonitorService = new Service.Implement.Api.ProblemCaseMonitorService();
            logInputDataService = new LogInputDataService();
            _updateCategoryService = new UpdateCategoryService();
            _antiFraudCenterService = new AntiFraudCenterService();
            _customerClassService = new SpecialCustomerClassService();
            _problemAccountManagementService = new ProblemAccountManagementService();
            _paClassificationService = new ProblemAccountClassificationService();
        }

        /// <summary>
        /// Place evidence for multiple customer
        /// </summary>
        /// <param name="custIds">List custId in string format, separated by comma</param>
        /// <param name="evidenceId">Evidence Id in CTS system</param>
        /// <returns>true: success; false: failed</returns>
        //[HttpPost]
        //[TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.PlaceEvidenceForMultipleCustAPI)]
        //public bool PlaceEvidenceMultipleByCustId(string custIds, int evidenceId)
        //{
        //    bool returnedResult = false;
        //    if (!string.IsNullOrEmpty(custIds))
        //    {
        //        try
        //        {
        //            logInputDataService.LogInputData(new LogInputViewModel
        //            {
        //                PlaceEvidences = new PlaceEvidencesViewModel()
        //                {
        //                    CustIds = custIds,
        //                    EvidenceId = evidenceId
        //                }
        //            }, FunctionId.PlaceEvidenceForMultiCustAPI);

        //            int userId = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"]);

        //            var result = _custEvidenceService.AddCustEvidenceMultipleByCustId(custIds, evidenceId, userId);

        //            if (string.IsNullOrEmpty(result.ErrorMessage) && result.Eivdences != null && result.Eivdences.Any())
        //            {
        //                var syncCustIds = Enumerable.Empty<long>();

        //                var paLosingClassification = _rescanClassificationService.GetProblemAccountLosing(result.Eivdences, FunctionId.PlaceSingleEvidence);

        //                var custEvidenceJson = JsonConvert.SerializeObject(paLosingClassification);

        //                var syncResult = _accountService.SyncCustEvidenceToCategory(custEvidenceJson, false, out syncCustIds);

        //                Parallel.Invoke(() =>
        //                {
        //                    _custEvidenceService.UpdateBetLimitOfFlaggedCustomer(custIds);
        //                },
        //                () =>
        //                {
        //                    if (syncResult.ErrorCode == 0 && syncCustIds != null && syncCustIds.Any())
        //                    {
        //                        _custClassificationService.PushDataToExternal(syncCustIds, new PushDataToExternalModel
        //                        {
        //                            ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
        //                            IsPushCCToSoon88 = true,
        //                            IsUpdateDangerToMainDB = true,
        //                            DangerLevelType = DangerLevel.ABI
        //                        });

        //                       _accountService.CheckNewPAAssociationCustomerForRollback(syncResult.CustInfo);
        //                    }
        //                },
        //                () =>
        //                {
        //                    if (syncResult.ErrorCode == 0 && syncCustIds != null && syncCustIds.Any())
        //                    {
        //                        _antiFraudCenterService.InsertAFCAccounts(syncCustIds, FromFlow.CTSAPI);
        //                    }
        //                });
        //            }

        //            if (!string.IsNullOrEmpty(result.ErrorMessage))
        //            {
        //                LogService.Error($"CTS Portal API - Add Customers Evidences exception: {result.ErrorMessage} - List custIds: {custIds} - EvidenceId: {evidenceId}");
        //            }

        //            returnedResult = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            var logMessage = new LogMessage(ex.Message)
        //                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
        //                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

        //            LogService.Error($"CTS Portal API - PlaceEvidence exception: {logMessage} - CustIds: {custIds} - EvidenceId: {evidenceId}");
        //        }
        //    }

        //    return returnedResult;
        //}

        /// <summary>
        /// Get flagged customers by level
        /// </summary>
        /// <param name="CustIdList">CustId in string format (saparate by comma) </param>
        /// <param name="AssociatedLevel"> Level 0, 1 or 2</param>
        /// <returns>A list of customer evidence in JSON format</returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetFlaggedCustomersByLevelAPI)]
        public JsonResult<List<CustomerEvidence>> GetFlaggedCustomersByLevel(string CustIdList, int AssociatedLevel)
        {
            try
            {
                int[] eviList = { -1 }; // set default for getting all
                var convertedCustIdList = CustIdList.Split(',').Select(a => Convert.ToInt32(a));
                var associationList = _associationService.GetFlaggedCustomerByLevel(convertedCustIdList, AssociatedLevel, eviList);
                var result = associationList.Distinct(new CustomerEvidenceComparer());

                return Json(result.ToList());
            }
            catch (Exception ex)
            {
                LogService.Error("Get Flagged Customers By Level exception: " + ex.Message);
                return Json(new List<CustomerEvidence>());
            }
        }

        /// <summary>
        /// Get flagged customers by level and evidence
        /// </summary>
        /// <param name="custIdList">CustId in string format (saparate by comma) </param>
        /// <param name="associatedLevel"> Level 0, 1 or 2</param>
        /// <param name="eviIdList"> Evidence in string format (saparate by comma) (</param>
        /// <returns>A list of customer evidence in JSON format</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetFlaggedCustByLvAndEvidenceAPI)]
        public JsonResult<List<CustomerEvidence>> GetFlaggedCustByLvlNEvi(string custIdList, int associatedLevel, string eviIdList)
        {
            try
            {
                var convertedEviList = eviIdList.Split(',').Select(a => Convert.ToInt32(a));
                var convertedCustIdList = custIdList.Split(',').Select(a => Convert.ToInt32(a));
                var associationList = _associationService.GetFlaggedCustomerByLevel(convertedCustIdList, associatedLevel, convertedEviList);
                var result = associationList.Distinct(new CustomerEvidenceComparer());

                return Json(result.ToList());
            }
            catch (Exception ex)
            {
                LogService.Error("Get Flagged Customers By Level exception: " + ex.Message);
                return Json(new List<CustomerEvidence>());
            }
        }

        /// <summary>
        /// Add evidences to customers
        /// </summary>
        /// <param name="customerEvidenceList"></param>
        /// <returns></returns>
        //[HttpPost]
        //[TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.AddCustomersEvidencesAPI)]
        //public bool AddCustomersEvidences(PlaceMultiCustomerEvidence customerEvidenceList)
        //{
        //    try
        //    {
        //        logInputDataService.LogInputData(new LogInputViewModel { PlaceMultiEvidences = customerEvidenceList }, FunctionId.PlaceMultiEvidencesForMultiCustAPI);

        //        int userId = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"]);

        //        var result = _evidenceService.AddCustomersEvidences(customerEvidenceList, userId);

        //        if (string.IsNullOrEmpty(result.ErrorMessage) && result.Eivdences != null && result.Eivdences.Any())
        //        {
        //            var syncCustIds = Enumerable.Empty<long>();

        //            var paLosingClassification = _rescanClassificationService.GetProblemAccountLosing(result.Eivdences, FunctionId.PlaceSingleEvidence);

        //            var custEvidenceJson = JsonConvert.SerializeObject(paLosingClassification);

        //            var syncResult = _accountService.SyncCustEvidenceToCategory(custEvidenceJson, false, out syncCustIds);

        //            Parallel.Invoke(() =>
        //            {
        //                _custEvidenceService.UpdateBetLimitOfFlaggedCustomer(customerEvidenceList.CustIdList);
        //            },
        //            () =>
        //            {
        //                if (syncResult.ErrorCode == 0 && syncCustIds != null && syncCustIds.Any())
        //                {
        //                    _custClassificationService.PushDataToExternal(syncCustIds, new PushDataToExternalModel
        //                    {
        //                        ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
        //                        IsPushCCToSoon88 = true,
        //                        IsUpdateDangerToMainDB = true,
        //                        DangerLevelType = DangerLevel.ABI
        //                    });

        //                    _accountService.CheckNewPAAssociationCustomerForRollback(syncResult.CustInfo);
        //                }
        //            },
        //            () =>
        //            {
        //                if (syncResult.ErrorCode == 0 && syncCustIds != null && syncCustIds.Any())
        //                {
        //                    _antiFraudCenterService.InsertAFCAccounts(syncCustIds, FromFlow.CTSAPI);
        //                }
        //            });
        //        }

        //        if (!string.IsNullOrEmpty(result.ErrorMessage))
        //        {
        //            LogService.Error($"CTS Portal API - Add Customers Evidences exception: {result.ErrorMessage} - List custIds: {customerEvidenceList.CustIdList} - List EvidenceCode: {customerEvidenceList.EvidenceCodeList}");
        //            return false;
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        var logMessage = new LogMessage(ex.Message)
        //            .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
        //            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

        //        LogService.Error($"CTS Portal API - Add Customers Evidences exception: {logMessage} - Data: {JsonConvert.SerializeObject(customerEvidenceList)}");
        //        return false;
        //    }
        //}

        /// <summary>
        /// Get Association data from MySQL
        /// </summary>
        /// <param name="custId"></param>
        /// <returns></returns>
        [AcceptVerbs("GetAssociationForExtAcc")]
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetAssociationForExtAccAPI)]
        public string GetAssociationForExtAcc(int custId)
        {
            string xmlString = string.Empty;
            try
            {
                var result = _associationService.GetAssociationForExtAcc(custId);

                if (result != null)
                {
                    var associationList = result.Select(a => new AssociationForExtAcc
                    {
                        CustId = a.CustId == null ? 0 : a.CustId.Value,
                        CustSubId = a.CustSubId,
                        AssociationDate = a.AssociationDate.ToString()
                    });

                    xmlString = XmlHelper.ToXmlString(associationList);
                }

                return xmlString;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationByDevice failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.AddSystemFormulaBetAccountAPI)]
        public JsonResult<InsertNAPProblemAccountResult> AddSystemFormulaBetAccount(TWProblemAccountParams dataParam)
        {
            var isWriteLogInfo = ConfigSettings.IsWriteLogInfo;

            string successListFromNAP = string.Empty;
            var result = string.Empty;
            var orgCustomerList = dataParam.CustomerList.Split(',').Select(u => new TWProblemAccountModel { Username = u });
            if (orgCustomerList.Count() <= 100)
            {
                try
                {
                    var customers = _problemAccountMonitorService.GetCustClassBySite(dataParam.CustomerList);
                    var perfCust = _problemAccountMonitorService.GetCustomerPerformance(customers?.Where(c => c.IsMonitorSite).Select(c => c.CustID)?.Distinct());
                    var fullData = (from oCust in orgCustomerList
                                    join cust in customers on oCust.Username equals cust.Username into temp
                                    from cust in temp.DefaultIfEmpty()
                                    join pCust in perfCust on cust.CustID equals pCust.CustID into temp2
                                    from pCust in temp2.DefaultIfEmpty()
                                    select new TWProblemAccountModel
                                    {
                                        CustID = cust.CustID,
                                        Username = oCust.Username,
                                        IsMonitorCategoryGroup = cust.IsMonitorCategoryGroup,
                                        IsMonitorSite = cust.IsMonitorSite,
                                        Winloss = pCust == null ? 0 : pCust.Winloss
                                    }).ToList();

                    var monitorList = fullData.Where(c => (c.Winloss < 0 && c.IsMonitorSite) || (c.Winloss >= 0 && c.IsMonitorSite && c.IsMonitorCategoryGroup));

                    var monitorListMoreData = monitorList.Select(c => new
                    {
                        CustID = c.CustID,
                        UserName = c.Username,
                        RoleID = dataParam.RoleId,
                        ReasonID = dataParam.ReasonId,
                        PreventActionID = dataParam.PreventActionId,
                        Comment = dataParam.Comment,
                        VoidTime = dataParam.VoidTime,
                        LimitAmount = dataParam.LimitAmount,
                        UserID = dataParam.UserId,
                        IP = dataParam.IP
                    });

                    var markPAList = fullData.Except(monitorList);

                    var monitorListMoreDataJson = JsonConvert.SerializeObject(monitorListMoreData);
                    if (isWriteLogInfo)
                    {
                        LogService.Info(monitorListMoreDataJson);
                    }

                    _problemAccountMonitorService.InsertSystemFormulaCust(monitorListMoreDataJson);

                    if (markPAList.Any())
                    {
                        //successListFromNAP = _problemAccountMonitorService.InsertProblemAccToNAP(string.Join(",", markPAList.Select(c => c.Username)), dataParam);
                        var pas = markPAList.Select(x => new InsertProblemAccountModel
                        {
                            CustID = x.CustID,
                            ReasonID = dataParam.ReasonId,                            
                            Remark = dataParam.Comment
                        });
                        
                        var isInsertPAResult = _paClassificationService.InsertProblemAccount(pas, MappingCategoryByType.Reason);

                        if (isInsertPAResult)
                        {
                            _problemAccountMonitorService.RemoveSystemFormulaBetCustomer(string.Join(",", markPAList.Select(c => c.CustID)));
                        }

                        if (isWriteLogInfo)
                        {
                            LogService.Info(string.Join(",", markPAList.Select(c => c.CustID)));
                        }
                    }

                    result = string.Join(",", monitorListMoreData.Select(c => c.UserName)) + successListFromNAP;
                }
                catch (Exception ex)
                {
                    LogService.Error("AddSystemFormulaBetAccount failed: "
                                        + "\nException" + ex.Message + ex.StackTrace);
                }
            }

            return Json(new InsertNAPProblemAccountResult { SuccessList = result });
        }

        /// <summary>
        /// Get association by custId and level
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="level">Level 1 for direct, level 2 for indirect</param>
        /// <returns>Association list in XML format</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetAssociationByCustIdAPI)]
        public string GetAssociationByCustId(int custId, int level)
        {
            var result = string.Empty;
            try
            {
                var spName = "GetAssociationByCustIdLevel" + level;
                result = DbSessionFactory.Execute<Association>(spName, new
                {
                    CustId = custId
                }).Data.ToXmlString();
            }
            catch (Exception ex)
            {
                LogService.Error("GetAssociationByCustId failed: " + ex.Message + ex.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// Get classification of customer
        /// </summary>
        /// <param name="custIdList"></param>
        /// <returns>Customer classification in XML format</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetCustomerClassificationAPI)]
        public string GetCustomerClassification(string custIdList)
        {
            var result = string.Empty;
            try
            {
                var custClassifications = _accountService.GetCustomerClassificationForAPI(custIdList, "50,200");
                result = custClassifications.Select(c => new CustomerClassification()
                {
                    CustID = c.CustID,
                    ParentCategoryName = c.ParentCategoryName,
                    CategoryName = c.CategoryName
                }).ToXmlString();
            }
            catch (Exception ex)
            {
                LogService.Error("GetCustomerClassification failed - Data: " + custIdList
                    + "\nException" + ex.Message + ex.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// Get classification of customer by Category's IDs
        /// </summary>
        /// <param name="categoryIDList"></param>
        /// <returns>Customer classification in XML format</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetCustomerClassificationByCategoryIDsAPI)]
        public string GetCustomerClassificationByCategoryIDs(string categoryIDList)
        {
            try
            {
                var custClassifications = _accountService.GetCustomerClassificationByCategory(categoryIDList);
                return custClassifications.Any()
                    ? custClassifications.Select(c => new CustomerClassification()
                    {
                        CustID = c.CustID,
                        ParentCategoryName = "",
                        CategoryName = c.CategoryName
                    }).ToXmlString()
                    : string.Empty;
            }
            catch (Exception ex)
            {
                LogService.Error("GetCustomerClassificationByCategoryIDs failed: " + ex.Message + ex.StackTrace);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get VVIP Customers
        /// </summary>
        /// <returns>Customer classification in XML format</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetVVIPCustomerAPI)]
        public string GetVVIPCustomer()
        {
            try
            {
                var custClassifications = _accountService.GetCustomerClassificationByCategory("210");
                return custClassifications.Any()
                    ? custClassifications.Select(c => new CustomerClassification()
                    {
                        CustID = c.CustID,
                        ParentCategoryName = "",
                        CategoryName = c.CategoryName
                    }).ToXmlString()
                    : string.Empty;
            }
            catch (Exception ex)
            {
                LogService.Error("GetCustomerClassificationByCategoryIDs failed: " + ex.Message + ex.StackTrace);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get class of customer
        /// </summary>
        /// <param name="custIdList"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns>Customer class in XML format</returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.CheckCustomerNotExistedAPI)]
        public IEnumerable<CTSCustomer> CheckCustomerNotExisted(string custIdList, DateTime fromDate, DateTime toDate)
        {
            try
            {
                return _accountService.CheckCustomerNotExisted(custIdList, fromDate, toDate);
            }
            catch (Exception ex)
            {
                LogService.Error("CheckCustomerNotExisted failed: " + ex.Message + ex.StackTrace);
            }

            return null;
        }

        #region Problem Account

        /// <summary>
        ///  Update Removed Problem Account into CTS
        /// </summary>
        /// <param name="paras">List custId in string format, separated by comma</param>
        /// <returns>true: success; false: failed</returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.RemoveProblemAccountsAPI)]
        public ApiResultModel RemoveProblemAccounts([FromBody] RemovePANapParamsModel paras)
        {
            try
            {
                var isWriteLogInfo = ConfigSettings.IsWriteLogInfo;

                if (isWriteLogInfo)
                    LogService.Info($"CTS Portal API - RemoveProblemAccounts endpoint - start {paras.CustIds}");

                var output = _problemAccountMonitorService.UpdateRemovedProblemAccountToCTS(paras.CustIds, paras.UserId);

                if (string.IsNullOrEmpty(output.ErrorMessage) && output.CustIds != null && output.CustIds.Any())
                {
                    if (isWriteLogInfo)
                        LogService.Info($"CTS Portal API - UpdateRemovedProblemAccountToCTS successful custIds:  {JsonConvert.SerializeObject(output.CustIds)}");

                    var custEviRemoved = _problemAccountManagementService.RemoveProblemAccountEvidence(paras.CustIds);

                    if (custEviRemoved != null && custEviRemoved.Any())
                    {
                        var custEviRemovedIds = string.Join(",", custEviRemoved);
                        _custEvidenceService.UpdateBetLimitOfUnflaggedCustomer(custEviRemovedIds);
                    }

                    _rescanClassificationService.RescanCustomerClassify(new RescanCustClassificationModel
                    {
                        CustIds = output.CustIds,
                        IsCheckPAFromNAP = false,
                        IsPushCCToSoon88 = true,
                        FunctionId = FunctionId.RemovePAAPI,
                        IsUpdateDangerToMainDB = true,
                        DangerLevelType = DangerLevel.ABI
                    });
                }
                else if (!string.IsNullOrEmpty(output.ErrorMessage))
                {
                    LogService.Error($"CTS Portal API - Remove Problem Account API - Rescan Problem Account failed: {output.ErrorMessage} - List custIds: {paras.CustIds}.");
                    return new ApiResultModel { ErrorMessage = string.Empty, StatusMessage = "fail" };
                }

                if (isWriteLogInfo)
                    LogService.Info($"CTS Portal API - RemoveProblemAccounts endpoint - end");

                return new ApiResultModel
                {
                    ErrorMessage = string.Empty,
                    StatusMessage = "success"
                };
            }
            catch (Exception ex)
            {
                LogService.Error($"CTS Portal API - RemoveProblemAccounts endpoint - Exception: {ex.Message} - List custIds: {paras.CustIds}.", ex);

                return new ApiResultModel
                {
                    ErrorMessage = ex.Message,
                    StatusMessage = "fail"
                };
            }
        }

        /// <summary>
        /// Update Category To CTS When PA Updated
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.UpdateCategoryToCTSWhenPAUpdatedAPI)]
        public ApiResultModel UpdateCategoryToCTSWhenPAUpdated([FromBody] UpdateCategoryParamsModel paras)
        {
            var result = new ApiResultModel
            {
                ErrorMessage = string.Empty,
                StatusMessage = "success"
            };

            try
            {
                if (paras != null && paras.CustInfos != null && paras.CustInfos.Any())
                {
                    var custIds = paras.CustInfos.Select(c => c.CustID)?.Distinct();

                    var probAccountLosing = _rescanClassificationService.GetProblemAccountLosing(custIds, FunctionId.UpdateCategory);

                    var updateCategoryResult = _updateCategoryService.UpdateCategoryToCTSWhenPAUpdated(paras, probAccountLosing);

                    if (updateCategoryResult != null && updateCategoryResult.Any())
                    {
                        var assCustForRollbacks = updateCategoryResult.Select(x => new CheckPACustomerAssociationForRollbackModel
                        {
                            CTSCustID = x.CTSCustID,
                            CustID = x.CustID
                        });

                        var listCustIdToSyncCC = assCustForRollbacks.Select(x => (long)x.CustID);

                        var options = new PushDataToExternalModel 
                        { 
                            ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize, 
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = true,
                            DangerLevelType = DangerLevel.ABI
                        };

                        var custEviUpdated = _problemAccountManagementService.UpdateProblemAccountEvidence(paras);

                        if (custEviUpdated != null && custEviUpdated.Any())
                        {
                            var custEviUpdatedIds = string.Join(",", custEviUpdated);
                            _custEvidenceService.UpdateBetLimitOfFlaggedCustomer(custEviUpdatedIds);
                        }

                        Parallel.Invoke(() =>
                        {
                            _rescanClassificationService.CheckNewPAAssociationCustomerForRollback(assCustForRollbacks);
                        },
                        () =>
                        {
                            _custClassificationService.PushDataToExternal(listCustIdToSyncCC, options);
                        },
                        () =>
                        {
                            _antiFraudCenterService.InsertAFCAccounts(listCustIdToSyncCC, FromFlow.CTSAPI);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"CTS Portal API - UpdateCategoryToCTSWhenPAUpdated endpoint - Exception: {ex.Message} with params {JsonConvert.SerializeObject(paras)}", ex);

                result = new ApiResultModel
                {
                    ErrorMessage = ex.Message,
                    StatusMessage = "fail"
                };
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Get customer class's categories
        /// </summary>
        /// <returns>A list of customer class's categories in JSON format</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetCustomerClassCategoryAPI)]
        public JsonResult<List<CategoryCustomerClassModel>> GetCustomerClassCategory()
        {
            try
            {
                var customerClassCategories = _categoryService.GetCustomerClassCategory().ToList();
                customerClassCategories.Add(
                    new CategoryCustomerClassModel
                    {
                        CategoryName = "VIP designated by licensee",
                        CategoryNameCN = "承租公司指定为VIP玩家",
                        CustomerClass = 99
                    }
                );

                return Json(customerClassCategories);
            }
            catch (Exception ex)
            {
                LogService.Error("Get Customer Class's Categories exception: " + ex.Message);
                return Json(new List<CategoryCustomerClassModel>());
            }
        }

        /// <summary>
        /// Get Subscriber info By Subscriber name
        /// </summary>
        /// <param name="subscriberName"></param>
        /// <returns>Subscriber Info</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetSubscriberInfoAPI)]
        public JsonResult<SubscriberInfo> GetSubscriberInfo(string subscriberName)
        {
            try
            {
                var subscriber = _subscriberService.GetSubscriberBySubscriberName(subscriberName);

                if (subscriber != null && subscriber.DCSStatus)
                {
                    return Json(subscriber);
                }
                else
                {
                    LogService.Error($"{subscriberName} is not integrated on CTS.");
                }

                return Json(new SubscriberInfo());
            }
            catch (Exception ex)
            {
                LogService.Error("Get Subscriber info exception: " + ex.Message);
                return Json(new SubscriberInfo());
            }
        }

        /// <summary>
        /// Get Association by CustId
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="associationType">1-Device, 2-Manual</param>
        /// <returns></returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetAssociationPerTypeAPI)]
        public string GetAssociationPerType(long custId, int associationType)
        {
            var result = string.Empty;
            var script = string.Empty;

            switch (associationType)
            {
                case 1:
                    script = "GetAssociationOneLevelByCustIdByDevice";
                    break;

                case 2:
                    script = "GetAssociationOneLevelByCustIdByManual";
                    break;

                default:
                    script = "GetAssociationOneLevelByCustIdByDevice";
                    break;
            }

            if (associationType == 3)
            {
                var listCustIds = new List<long>() { custId };
                var associations = _associationService.GetAssociationByIP(listCustIds.Join(a => a, ","));
                if (associations.Any())
                {
                    return result = JsonConvert.SerializeObject(associations.Select(c => new
                    {
                        c.CustID
                    }));
                }
            }
            else
            {
                try
                {
                    var custfromQuery = DbSessionFactory.Execute<GraphCustomer>("GetCustomerByCustId", new
                    {
                        CustID = custId,
                        CustSubID = 0
                    });

                    if (custfromQuery.Data.Any())
                    {
                        var cust = custfromQuery.Data.First();
                        var queryResult = DbSessionFactory.Execute<AssociatedAccountPagingModel>(script, new
                        {
                            CTSCustID = cust.CTSCustId,
                            AssStatus = 0,
                            IsAssReport = 1,
                            AssCreatedFrom = DateTime.Now.AddYears(-3),
                            AssCreatedTo = DateTime.Now,
                            AccountStatuses = new List<int>(),
                            SubscriberIDs = new List<int>(),
                            Skip = 0,
                            Pagesize = 1000
                        }).Data.FirstOrDefault();

                        if (queryResult != null)
                        {
                            var listAss = queryResult.AssociationList.Select(ass => new
                            {
                                ass.Username,
                                //AssDate = ass.AssociationDateGMT0
                            });

                            result = JsonConvert.SerializeObject(listAss);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error("GetAssociationPerType failed: " + ex.Message + ex.StackTrace + "\n"
                            + ex.InnerException.Message + "\n"
                            + ex.InnerException.Source + ((Neo4j.Driver.Neo4jException)ex.InnerException).Code
                            + ex.InnerException.StackTrace);
                }
            }

            return result;
        }

        /// Get list Problem Case
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetProblemCaseAPI)]
        public JsonResult<List<ProblemCaseModel>> GetProblemCase()
        {
            try
            {
                var problemCases = _problemCaseMonitorService.GetProlemCase(12, true);

                return Json(problemCases.ToList());
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                return Json(new List<ProblemCaseModel>());
            }
        }

        /// <summary>
        /// Get list Problem Case Monitor
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetProblemCaseMonitorAPI)]
        public JsonResult<List<ProblemCaseMonitorViewModel>> GetProblemCaseMonitor([FromBody] ProblemCaseMonitorParamsModel paras)
        {
            try
            {
                var problemCases = new List<ProblemCaseMonitorViewModel>();

                var problemCaseData = _problemCaseMonitorService.GetProblemCaseMonitor(paras);

                if (problemCaseData != null && problemCaseData.Any())
                {
                    var problemCaseDataGroup = problemCaseData.GroupBy(x => new { x.CTSCustID, x.UserName, x.SubscriberName }, (key, g) => new
                    {
                        CTSCustID = key.CTSCustID,
                        UserName = key.UserName,
                        SubscriberName = key.SubscriberName,
                        Items = g
                    });

                    problemCases = problemCaseDataGroup.Select(x => new ProblemCaseMonitorViewModel
                    {
                        CTSCustId = x.CTSCustID,
                        UserName = x.UserName,
                        SubscriberName = x.SubscriberName,
                        IsRobot = x.Items.Any(y => y.ProblemCaseID == Consts.ProblemCase_ItemID_Robot),
                        IsSuspiciousAssociation = x.Items.Any(y => y.ProblemCaseID == Consts.ProblemCase_ItemID_SuspiciousAssociation),
                        IsProPunter = x.Items.Any(y => y.ProblemCaseID == Consts.ProblemCase_ItemID_ProPunter)
                    }).OrderBy(x => x.UserName).ToList();
                }

                return Json(problemCases);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                return Json(new List<ProblemCaseMonitorViewModel>());
            }
        }

        /// <summary>
        /// Insert special CC (build firstly for Haifa)
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.ForceSpecialCCAPI)]
        public bool ForceSpecialCC([FromBody] ForceSpecialCustomerClassModel paras)
        {
            var result = true;
            try
            {
                var userId = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultUserIDForForceCC"]);
                var resultInsert = new ResultInsertRemoveViewModel
                {
                    ErrorCode = 0,
                    ErrorMessage = string.Empty
                };

                var resultInsertToCTS = _customerClassService.InsertCustomerClass(paras.Username, paras.CustomerClass, paras.IsAffectedToAssociation, userId);

                if (string.IsNullOrEmpty(resultInsertToCTS.OpErrorMessage))
                {
                    if (resultInsertToCTS.CustomerClassList != null && resultInsertToCTS.CustomerClassList.Any())
                    {
                        var custIdsChunk = resultInsertToCTS.CustomerClassList.Chunk(ConfigSettings.PushCCToExternalChunkSize);

                        foreach (var custs in custIdsChunk)
                        {
                            // Update CC in table custinfo to MainDB
                            var isSuccessToMain = _custClassificationService.RetryPushCCToMainDB(custs, 2);

                            if (isSuccessToMain)
                            {
                                var updateApplyCCs = custs.Select(x => new { x.CustId, x.ScannedTime });

                                var jsonUpdateApplyCCs = JsonConvert.SerializeObject(updateApplyCCs);

                                // Update IsAppliedCC = 1 in CTS
                                _custClassificationService.UpdateAppliedCustomerClassification(jsonUpdateApplyCCs);
                            }
                        }
                    }
                }
                else
                {
                    resultInsert.ErrorCode = -1;
                    resultInsert.ErrorMessage = resultInsertToCTS.OpErrorMessage;

                    result = false;
                    LogService.Error("ForceSpecialCC failed: " + resultInsert.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                LogService.Error("ForceSpecialCC exception: " + ex.Message);
            }

            return result;
        }
    }
}