namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.TWProblemAccountDetection;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.API.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Model.API.UpdateCategoryWhenPAUpdated;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.Model.LogInputData;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Implement.Api;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using Fanex.NAP.CTS.ViewModel.API.ProblemCaseMonitor;
    using Fanex.NAP.SPU.Graph.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
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
        private readonly IRescanCustomerClassificationService _customerClassifyService;
        private readonly ICustomerClassificationService _custClassificationService;
        private readonly ISubscriberService _subscriberService;
        private readonly IProblemCaseMonitorService _problemCaseMonitorService;
        private readonly ILogInputDataService logInputDataService;
        private readonly IUpdateCategoryService _updateCategoryService;

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
            _customerClassifyService = new RescanCustomerClassificationService();
            _custClassificationService = new CustomerClassificationService();
            _subscriberService = new SubscriberService();
            _problemCaseMonitorService = new Service.Implement.Api.ProblemCaseMonitorService();
            logInputDataService = new LogInputDataService();
            _updateCategoryService = new UpdateCategoryService();
        }

        ///// <summary>
        ///// Place Evidence on CTS
        ///// </summary>
        ///// <param name="evidence"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public bool PlaceEvidence(UpdatePlaceEvidenceModel evidence)
        //{
        //    try
        //    {
        //        _accountService.ProcessPlaceEvidenceByCustId(evidence);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Error("PlaceEvidence exception: " + ex.Message);
        //        return false;
        //    }
        //}

        /// <summary>
        /// Place evidence for multiple customer
        /// </summary>
        /// <param name="custIds">List custId in string format, separated by comma</param>
        /// <param name="evidenceId">Evidence Id in CTS system</param>
        /// <returns>true: success; false: failed</returns>
        [HttpPost]
        public bool PlaceEvidenceMultipleByCustId(string custIds, int evidenceId)
        {
            bool returnedResult = false;
            if (!string.IsNullOrEmpty(custIds))
            {
                try
                {
                    logInputDataService.LogInputData(new LogInputViewModel
                    {
                        PlaceEvidences = new PlaceEvidencesViewModel()
                        {
                            CustIds = custIds,
                            EvidenceId = evidenceId
                        }
                    }, FunctionId.PlaceEvidenceForMultiCustAPI);

                    int userId = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"]);

                    var result = _custEvidenceService.AddCustEvidenceMultipleByCustId(custIds, evidenceId, userId);

                    if (string.IsNullOrEmpty(result.ErrorMessage) && result.Eivdences != null && result.Eivdences.Any())
                    {
                        var syncCustIds = Enumerable.Empty<long>();

                        Parallel.Invoke(() =>
                        {
                            _custEvidenceService.UpdateBetLimitOfFlaggedCustomer(custIds);
                        },
                        () =>
                        {
                            var syncResult = _accountService.SyncCustEvidenceToCategory(JsonConvert.SerializeObject(result.Eivdences), false, out syncCustIds);

                            if (syncResult.ErrorCode == 0 && syncCustIds != null && syncCustIds.Any())
                            {
                                _custClassificationService.PushDataToExternal(syncCustIds, new PushDataToExternalModel
                                {
                                    ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                                    IsPushCCToSoon88 = true,
                                    IsUpdateDangerToMainDB = false
                                });

                                var hashCustInfos = syncResult.OutParam["custInfos"];

                                if (hashCustInfos != null)
                                {
                                    var custInfos = (IEnumerable<NewPAAssociationCustomerForRollbackModel>)hashCustInfos;

                                    _accountService.CheckNewPAAssociationCustomerForRollback(custInfos);
                                }
                            }
                        });
                    }

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        LogService.Error($"CTS Portal API - Add Customers Evidences exception: {result.ErrorMessage} - List custIds: {custIds} - EvidenceId: {evidenceId}");
                    }

                    returnedResult = true;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                            .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"CTS Portal API - PlaceEvidence exception: {logMessage} - CustIds: {custIds} - EvidenceId: {evidenceId}");
                }
            }

            return returnedResult;
        }

        /// <summary>
        /// Get flagged customers by level
        /// </summary>
        /// <param name="CustIdList">CustId in string format (saparate by comma) </param>
        /// <param name="AssociatedLevel"> Level 0, 1 or 2</param>
        /// <returns>A list of customer evidence in JSON format</returns>
        [HttpPost]
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
        [HttpPost]
        public bool AddCustomersEvidences(PlaceMultiCustomerEvidence customerEvidenceList)
        {
            try
            {
                logInputDataService.LogInputData(new LogInputViewModel { PlaceMultiEvidences = customerEvidenceList }, FunctionId.PlaceMultiEvidencesForMultiCustAPI);

                var result = _evidenceService.AddCustomersEvidences(customerEvidenceList);

                if (string.IsNullOrEmpty(result.ErrorMessage) && result.Eivdences != null && result.Eivdences.Any())
                {
                    Parallel.Invoke(() =>
                    {
                        _custEvidenceService.UpdateBetLimitOfFlaggedCustomer(customerEvidenceList.CustIdList);
                    },
                    () =>
                    {
                        var syncCustIds = Enumerable.Empty<long>();

                        var syncResult = _accountService.SyncCustEvidenceToCategory(JsonConvert.SerializeObject(result.Eivdences), false, out syncCustIds);

                        if (syncResult.ErrorCode == 0 && syncCustIds != null && syncCustIds.Any())
                        {
                            _custClassificationService.PushDataToExternal(syncCustIds, new PushDataToExternalModel
                            {
                                ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                                IsPushCCToSoon88 = true,
                                IsUpdateDangerToMainDB = false
                            });

                            var hashCustInfos = syncResult.OutParam["custInfos"];

                            if (hashCustInfos != null)
                            {
                                var custInfos = (IEnumerable<NewPAAssociationCustomerForRollbackModel>)hashCustInfos;

                                _accountService.CheckNewPAAssociationCustomerForRollback(custInfos);
                            }
                        }
                    });
                }

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    LogService.Error($"CTS Portal API - Add Customers Evidences exception: {result.ErrorMessage} - List custIds: {customerEvidenceList.CustIdList} - List EvidenceCode: {customerEvidenceList.EvidenceCodeList}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"CTS Portal API - Add Customers Evidences exception: {logMessage} - Data: {JsonConvert.SerializeObject(customerEvidenceList)}");
                return false;
            }
        }

        /// <summary>
        /// Get Association data from MySQL
        /// </summary>
        /// <param name="custId"></param>
        /// <returns></returns>
        [AcceptVerbs("GetAssociationForExtAcc")]
        [HttpGet]
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
        public void AddSystemFormulaBetAccount(NapProblemAccInsertCriteria dataParam)
        {
            var orgCustomerList = dataParam.CustomerList.Split(',').Select(u => new TWProblemAccountModel { Username = u });
            var customers = _problemAccountMonitorService.GetCustClassBySite(dataParam.CustomerList);
            var perfCust = _problemAccountMonitorService.GetCustomerPerformance(string.Join(",", customers.Where(c => c.IsMonitorSite).Select(c => c.CustID)));
            var fullData = (from oCust in orgCustomerList
                        join cust in customers on oCust.Username equals cust.Username into temp from cust in temp.DefaultIfEmpty()
                        join pCust in perfCust on cust.CustID equals pCust.CustID into temp2 from pCust in temp2.DefaultIfEmpty()
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
                LimitAcount = dataParam.LimitAmount,
                UserID = dataParam.UserId,
                IP = dataParam.IP
            });

            var markPAList = fullData.Except(monitorList);

            var monitorListMoreDataJson = JsonConvert.SerializeObject(monitorListMoreData);
            LogService.Info(monitorListMoreDataJson);
            _problemAccountMonitorService.InsertSystemFormulaCust(monitorListMoreDataJson);

            if (markPAList.Any())
            {
                _problemAccountMonitorService.InsertProblemAccToNAP(string.Join(",", markPAList.Select(c => c.Username)), dataParam);
                _problemAccountMonitorService.RemoveSystemFormulaBetCustomer(string.Join(",", markPAList.Select(c => c.CustID)));
                LogService.Info(string.Join(",", markPAList.Select(c => c.CustID)));
            }
        }

        ///This code for GraphDB - Neo4j

        //[AcceptVerbs("GetAssociationForExtAcc")]
        //[HttpGet]
        //public string GetAssociationForExtAcc(int custId)
        //{
        //    var xmlString = string.Empty;
        //    try
        //    {
        //        var cust = DbSessionFactory.Execute<GraphCustomer>("GetCustomerByCustId", new
        //        {
        //            CustID = custId,
        //            CustSubID = 0
        //        }).Data.FirstOrDefault();

        //        if (cust != null)
        //        {
        //            var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelForExtAcc", new
        //            {
        //                CTSCustID = cust.CTSCustId,
        //                AssStatus = 1,
        //                Skip = 0,
        //                Pagesize = 100000
        //            }).Data.FirstOrDefault();

        //            if (result != null)
        //            {
        //                var associationList = result.AssociationList.Select(a => new AssociationForExtAcc
        //                {
        //                    CustId = a.CustId == null ? 0 : a.CustId.Value,
        //                    CustSubId = a.CustSubId,
        //                    AssociationDate = a.AssociationDateFromGraphDB.ToString()
        //                });

        //                xmlString = XmlHelper.ToXmlString(associationList);
        //            }
        //        }

        //        return xmlString;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Error("GetDirectAssociationByDevice failed: " + ex.Message + ex.StackTrace);
        //        throw;
        //    }
        //}

        /// <summary>
        /// Get association by custId and level
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="level">Level 1 for direct, level 2 for indirect</param>
        /// <returns>Association list in XML format</returns>
        [HttpGet]
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

        /// <summary>
        /// This controller is only for testing
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void Test()
        {
            var ctsClient = new WebClient();
            ctsClient.QueryString.Add("CustId", "43913195");
            var e = ctsClient.DownloadString("http://ha.nexdev.net:7723/api/cts/GetAssociationForExtAcc");

            var a = ApiHelper.Get<string>("http://ha.nexdev.net:7723/api/cts/GetAssociationForExtAcc?CustId=43913195").Result;
        }

        #region Problem Account

        /// <summary>
        ///  Update Removed Problem Account into CTS
        /// </summary>
        /// <param name="custIds">List custId in string format, separated by comma</param>
        /// <returns>true: success; false: failed</returns>
        [HttpPost]
        public ApiResultModel RemoveProblemAccounts([FromBody] string custIds)
        {
            try
            {
                var isWriteLogInfo = ConfigSettings.IsWriteLogInfo;

                if (isWriteLogInfo)
                    LogService.Info($"CTS Portal API - RemoveProblemAccounts endpoint - start {custIds}");

                var output = _problemAccountMonitorService.UpdateRemovedProblemAccountToCTS(custIds);

                if (string.IsNullOrEmpty(output.ErrorMessage) && output.CustIds != null && output.CustIds.Any())
                {
                    if (isWriteLogInfo)
                        LogService.Info($"CTS Portal API - UpdateRemovedProblemAccountToCTS successful custIds:  {JsonConvert.SerializeObject(output.CustIds)}");

                    _customerClassifyService.RescanCustomerClassify(new RescanCustClassificationModel
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
                    LogService.Error($"CTS Portal API - Remove Problem Account API - Rescan Problem Account failed: {output.ErrorMessage} - List custIds: {custIds}.");
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
                LogService.Error($"CTS Portal API - RemoveProblemAccounts endpoint - Exception: {ex.Message} - List custIds: {custIds}.", ex);

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
        public ApiResultModel UpdateCategoryToCTSWhenPAUpdated([FromBody]UpdateCategoryParamsModel paras)
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
                    var updateCategoryResult = _updateCategoryService.UpdateCategoryToCTSWhenPAUpdated(paras);

                    if (updateCategoryResult != null && updateCategoryResult.Any())
                    {
                        var assCustForRollbacks = updateCategoryResult.Select(x => new NewPAAssociationCustomerForRollbackModel
                        {
                            CTSCustID = x.CTSCustID,
                            CustID = x.CustID
                        });

                        var listCustIdToSyncCC = assCustForRollbacks.Select(x => (long)x.CustID);

                        var options = new PushDataToExternalModel { ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize, IsPushCCToSoon88 = true };

                        _customerClassifyService.CheckNewPAAssociationCustomerForRollback(assCustForRollbacks);

                        _custClassificationService.PushDataToExternal(listCustIdToSyncCC, options);
                    }
                }
            }
            catch (Exception ex)
            {
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
                        var queryResult = DbSessionFactory.Execute<GraphAssociationPaging>(script, new
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
    }
}