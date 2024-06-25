namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Criteria.CustomerClassification.ProblemAccount;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.ProblemAccount;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class ProblemAccountClassificationService : BaseCustomerClassificationService, IProblemAccountClassificationService
    {
        private readonly IDynamicRepository _repository;
        private readonly ICustomerClassificationService _ccService;
        private readonly IDataLogTrackingServices dataLogTrackingServices;

        public ProblemAccountClassificationService()
        {
            _repository = new DynamicRepository();
            _ccService = new CustomerClassificationService();
            dataLogTrackingServices = new DataLogTrackingServices();
        }        

        public DBResult InsertProblemAccountPackage(IEnumerable<InsertProblemAccountModel> problemAccounts, InsertPA_ByType mappingCategoryByType, string serviceName, FromFlow fromFlow, bool isToQueue)
        {
            try
            {
                if (problemAccounts.Any())
                {
                    var custIds = problemAccounts.Select(c => c.CustID)?.Distinct();

                    var paLosing = GetProblemAccountLosingClassification(custIds, FunctionId.InsertPAM);

                    var paClassification = MergeProblemAccountClassification(problemAccounts, paLosing);

                    var insertPAResult = InsertProblemAccounts(paClassification, isToQueue);

                    if (insertPAResult.CustInfos != null && insertPAResult.CustInfos.Any())
                    {
                        var insertPACustIDs = insertPAResult.CustInfos.Select(x => x.CustID).Distinct();

                        CheckNewPAAssociationCustomerForRollback(JsonConvert.SerializeObject(insertPAResult.CustInfos));
                        
                        UpdateBetLimitOfFlaggedCustomer(insertPACustIDs);

                        _ccService.PushDataToExternalPackge(insertPACustIDs, new PushDataToExternalModel
                        {
                            ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = true,
                            IsSyncAFC = ConfigSettings.IsSyncAFCData
                        }, fromFlow);
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                dataLogTrackingServices.LogErrorData(ex, serviceName);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        //public DBResult InsertProblemAccountPackage(IEnumerable<InsertProblemAccountModel> problemAccounts, InsertPA_ByType mappingCategoryByType, string serviceName, FromFlow fromFlow, bool isToQueue)
        //{
        //    try
        //    {
        //        if (problemAccounts.Any())
        //        {
        //            var custIds = problemAccounts.Select(c => c.CustID)?.Distinct();

        //            var probAccountLosing = GetProblemAccountLosingClassification(custIds, FunctionId.InsertPAM);

        //            var custInfo = InsertProblemAccount(problemAccounts, probAccountLosing, mappingCategoryByType, isToQueue);

        //            if (custInfo != null && custInfo.Any())
        //            {
        //                InsertPALogics(custInfo);

        //                _ccService.PushDataToExternalPackge(custInfo.Select(c => c.CustID).Distinct(), new PushDataToExternalModel
        //                {
        //                    ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
        //                    IsPushCCToSoon88 = true,
        //                    IsUpdateDangerToMainDB = true,
        //                    IsSyncAFC = ConfigSettings.IsSyncAFCData
        //                }, fromFlow);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var logMessage = new LogMessage(ex.Message)
        //                          .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
        //                          .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

        //        LogService.Error(logMessage);

        //        dataLogTrackingServices.LogErrorData(ex, serviceName);

        //        return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
        //    }

        //    return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        //}

        //public IEnumerable<CustInfoModel> InsertProblemAccountLogics(IEnumerable<InsertProblemAccountModel> problemAccounts, FunctionId funcId, InsertPA_ByType mappingCategoryByType, bool isToQueue)
        //{
        //    if (problemAccounts.Any())
        //    {
        //        var custIds = problemAccounts.Select(c => c.CustID)?.Distinct();

        //        var probAccountLosing = GetProblemAccountLosingClassification(custIds, funcId);

        //        var custInfo = InsertProblemAccount(problemAccounts, probAccountLosing, mappingCategoryByType, isToQueue);

        //        if (custInfo != null && custInfo.Any())
        //        {
        //            InsertPALogics(custInfo);

        //            return custInfo;
        //        }
        //    }

        //    return Enumerable.Empty<CustInfoModel>();
        //}

        public ResultInsertCustomerClassificationModel InsertProblemAccounts(IEnumerable<InsertProblemAccountModel> problemAccountModels, bool isToQueue)
        {
            var modelInsert = new InsertCustomerClassificationModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(problemAccountModels),
                IsToQueue = isToQueue
            };

            return base.InsertCustomerClassification(CCInsertInputFlow.ProblemAccount, modelInsert);      
        }

        public ResultInsertCustomerClassificationModel InsertRobots(IEnumerable<InsertRobotModel> robotModels)
        {
            var modelInsert = new InsertCustomerClassificationModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(robotModels)
            };

            return base.InsertCustomerClassification(CCInsertInputFlow.Robot, modelInsert);
        }

        public IEnumerable<ExistedProblemAccountModel> GetExistedProblemAccounts(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var criteria = new ProblemAccountGetCriteria()
                {
                    CustIds = string.Join(",", custIds.Distinct())
                };

                return _repository.Fetch<ExistedProblemAccountModel>(criteria) ?? Enumerable.Empty<ExistedProblemAccountModel>();
            }

            return Enumerable.Empty<ExistedProblemAccountModel>();
        }

        public IEnumerable<ProblemAccountLosingClassificationModel> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId)
        {
            if (custIds != null && custIds.Any())
            {
                var criteria = new ProblemAccountLosingClassificationGetCriteria()
                {
                    ListCustIds = string.Join(",", custIds.Distinct()),
                    FunctionId = (int)funcId
                };

                return _repository.Fetch<ProblemAccountLosingClassificationModel>(criteria) ?? Enumerable.Empty<ProblemAccountLosingClassificationModel>();
            }

            return Enumerable.Empty<ProblemAccountLosingClassificationModel>();
        }

        public IEnumerable<InsertRobotModel> GetExistedTWRobotUser(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var criteria = new GetExistedTWRobotUserCriteria
                {
                    CustIds = string.Join(",", custIds)
                };

                var robotUsers = _repository.Fetch<RobotUserModel>(criteria);

                if (robotUsers != null && robotUsers.Any())
                {
                    return robotUsers.Select(c => new InsertRobotModel()
                    {
                        CustID = c.CustId,
                        RobotCounter = c.Counter,
                        IsFromTW = true,
                        IsDirect = true,
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"])
                    });
                }
            }

            return Enumerable.Empty<InsertRobotModel>();
        }

        public IEnumerable<CustInfoModel> InsertRobotAccountPackge(IEnumerable<InsertRobotModel> robotAccounts, bool isRescan)
        {
            if (robotAccounts != null && robotAccounts.Any())
            {
                var custIds = robotAccounts.Select(x => x.CustID).Distinct();

                var paLosing = GetProblemAccountLosingClassification(custIds, FunctionId.InsertPAM);

                var robotClassification = MergeRobotAccountClassification(robotAccounts, paLosing);

                var customerInfo = InsertRobotAccount(JsonConvert.SerializeObject(robotClassification), isRescan);

                CheckNewPAAssociationCustomerForRollback(JsonConvert.SerializeObject(customerInfo));

                return customerInfo;
            }

            return Enumerable.Empty<CustInfoModel>();
        }

        public UnmarkPAResultModel UnmarkProblemAccount(IEnumerable<UnmarkProblemAccountModel> paUnmark, bool isFromQueue)
        {
            var criteria = new ProblemAccountUmmarkCriteria
            {
                ProblemAccountJson = JsonConvert.SerializeObject(paUnmark),
                IsFromQueue = isFromQueue
            };

            var custInfos = _repository.Fetch<CustInfoModel>(criteria);

            return new UnmarkPAResultModel
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMessage = criteria.ErrorMessage,
                CustomerInfo = custInfos
            };
        }

        #region Private Methods

        private IEnumerable<CustInfoModel> InsertProblemAccount(IEnumerable<InsertProblemAccountModel> problemAccounts
            , IEnumerable<ProblemAccountLosingClassificationModel> paLosing
            , InsertPA_ByType insertByType
            , bool isToQueue)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var paClassification = MergeProblemAccountClassification(problemAccounts, paLosing);

                return InsertProblemAccount(JsonConvert.SerializeObject(paClassification), insertByType, isToQueue);
            }

            return Enumerable.Empty<CustInfoModel>();
        }

        private void InsertPALogics(IEnumerable<CustInfoModel> customerInfo)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                CheckNewPAAssociationCustomerForRollback(JsonConvert.SerializeObject(customerInfo));

                var custIds = customerInfo.Where(x=>x.CategoryType == 1).Select(x => x.CustID).Distinct();

                UpdateBetLimitOfFlaggedCustomer(custIds);
            }
        }

        private IEnumerable<InsertRobotModel> MergeRobotAccountClassification(IEnumerable<InsertRobotModel> robotAccounts, IEnumerable<ProblemAccountLosingClassificationModel> paLosing)
        {
            if (paLosing != null && paLosing.Any())
            {
                return robotAccounts.GroupJoin(paLosing,
                                                ro => ro.CustID,
                                                pl => pl.CustId,
                                                (ro, pl) => new { ro, pl })
                                                .SelectMany(group => group.pl.DefaultIfEmpty(),
                                                            (group, itemLosing) => new InsertRobotModel
                                                            {
                                                                CustID = group.ro.CustID,
                                                                CategoryID = group.ro.CategoryID,
                                                                RobotCounter = group.ro.RobotCounter,
                                                                Remark = group.ro.Remark,
                                                                CreatedBy = group.ro.CreatedBy,

                                                                IsFromTW = group.ro.IsFromTW,
                                                                IsFromCTS = group.ro.IsFromCTS,
                                                                IsFromAI = group.ro.IsFromAI,
                                                                IsFromImperva = group.ro.IsFromImperva,
                                                                IsFromTVS = group.ro.IsFromTVS,
                                                                TVSRequestID = group.ro.TVSRequestID,
                                                                IsDirect = group.ro.IsDirect,
                                                                IsParlay = group.ro.IsParlay,
                                                                SportType = group.ro.SportType,
                                                                IssueTypeID = group.ro.IssueTypeID,

                                                                TurnoverRM = itemLosing?.TurnoverRM,
                                                                WinlossRM = itemLosing?.WinlossRM,
                                                                WinlossStatus = itemLosing?.WinlossStatus,
                                                                BetCount = itemLosing?.BetCount,
                                                                ActiveDays = itemLosing?.ActiveDays
                                                            });
            }

            return robotAccounts;
        }

        private IEnumerable<InsertProblemAccountModel> MergeProblemAccountClassification(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                return problemAccounts.GroupJoin(probAccountLosing ?? Enumerable.Empty<ProblemAccountLosingClassificationModel>(),
                                            pa => pa.CustID,
                                            pl => pl.CustId,
                                            (pa, pl) => new { pa, pl })
                                            .SelectMany(
                                                 group => group.pl.DefaultIfEmpty(),
                                                 (group, temp) => new InsertProblemAccountModel
                                                 {
                                                     CustID = group.pa.CustID,
                                                     CTSCustID = group.pa.CTSCustID,
                                                     RoleID = group.pa.RoleID,
                                                     SubscriberID = group.pa.SubscriberID,
                                                     CategoryID = group.pa.CategoryID,
                                                     CategoryGroup = group.pa.CategoryGroup,
                                                     IsLicensee = group.pa.IsLicensee,
                                                     CreatedBy = group.pa.CreatedBy,
                                                     LastModifiedDate = group.pa.LastModifiedDate,
                                                     Remark = group.pa.Remark,

                                                     IsDirectPA = group.pa.IsDirectPA,
                                                     IsFromAI = group.pa.IsFromAI,
                                                     IsFromCTS = group.pa.IsFromCTS,
                                                     IsFromTVS = group.pa.IsFromTVS,
                                                     TVSRequestID = group.pa.TVSRequestID,
                                                     IsFromTW = group.pa.IsFromTW,
                                                     IsDataChanged = group.pa.IsDataChanged,
                                                     ReasonID = group.pa.ReasonID,
                                                     IsParlay = group.pa.IsParlay,
                                                     SportType = group.pa.SportType,
                                                     IssueTypeID = group.pa.IssueTypeID,

                                                     BetCount = temp?.BetCount,
                                                     ActiveDays = temp?.ActiveDays,
                                                     TurnoverRM = temp?.TurnoverRM,
                                                     WinlossRM = temp?.WinlossRM,
                                                     WinlossStatus = temp?.WinlossStatus
                                                 });
            }

            return Enumerable.Empty<InsertProblemAccountModel>();
        }

        private IEnumerable<CustInfoModel> InsertProblemAccount(string problemAccountJson, InsertPA_ByType inputType, bool isToQueue)
        {
            var criteria = new ProblemAccountMarkCriteria
            {
                ProblemAccountJson = problemAccountJson,
                IsToQueue = isToQueue,
                InputType = Enum.GetName(typeof(InsertPA_ByType), inputType)
            };

            return _repository.Fetch<CustInfoModel>(criteria) ?? Enumerable.Empty<CustInfoModel>();
        }

        public void RemoveOldProblemAccount(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var criteria = new OldProblemAccountRemoveCriteria
                {
                    CustId = string.Join(",", custIds)
                };

                _repository.Execute(criteria);
            }
        }

        public void UpdateBetLimitOfFlaggedCustomer(IEnumerable<long> custId)
        {
            if (custId != null && custId.Any())
            {
                var criteria = new BetLimitByEvidenceUpdateCriteria
                {
                    CustIDList = string.Join(",", custId)
                };

                _repository.Execute(criteria);
            }
        }

        public void UpdateBetLimitOfUnflaggedCustomer(IEnumerable<long> custId)
        {
            if (custId != null && custId.Any())
            {
                var criteria = new BetLimitByEvidenceRemoveCriteria
                {
                    CustIDList = string.Join(",", custId)
                };

                _repository.Execute(criteria);
            }
        }

        private void CheckNewPAAssociationCustomerForRollback(string custInfoJson)
        {
            var criteria = new CheckNewPAAssociationCustomerForRollbackCriteria
            {
                CustInfoJson = custInfoJson
            };

            _repository.Execute(criteria);
        }

        private IEnumerable<CustInfoModel> InsertRobotAccount(string robotAccountJson, bool isRescan)
        {
            var criteria = new RobotAccountInsertCriteria
            {
                CustInfo = robotAccountJson,
                IsRescan = isRescan
            };

            return _repository.Fetch<CustInfoModel>(criteria) ?? Enumerable.Empty<CustInfoModel>();
        }

        #endregion Private Methods

    }
}