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

    public class ProblemAccountClassificationService : IProblemAccountClassificationService
    {
        private readonly IDynamicRepository _repository;
        private readonly ICustomerClassificationService _ccService;
 

        public ProblemAccountClassificationService()
        {
            _repository = new DynamicRepository();
            _ccService = new CustomerClassificationService();
        }

        public DBResult InsertProblemAccountPackage(IEnumerable<InsertProblemAccountModel> problemAccounts, InsertPA_ByType mappingCategoryByType, string serviceName, FromFlow fromFlow)
        {
            try
            {
                if (problemAccounts.Any())
                {
                    var custIds = problemAccounts.Select(c => c.CustID)?.Distinct();

                    var probAccountLosing = GetProblemAccountLosingClassification(custIds, FunctionId.InsertPAM);

                    var result = InsertProblemAccount(problemAccounts, probAccountLosing, mappingCategoryByType, serviceName, false);

                    if (result.ErrorCode == 0 && result.CustomerInfo != null && result.CustomerInfo.Any())
                    {
                        InsertPALogics(result.CustomerInfo);

                        _ccService.PushDataToExternalPackge(result.CustomerInfo.Select(c => c.CustID).Distinct(), new PushDataToExternalModel
                        {
                            ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = true
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

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public InsertPAResultModel InsertProblemAccountLogics(IEnumerable<InsertProblemAccountModel> problemAccounts
            , IEnumerable<ProblemAccountLosingClassificationModel> paLosing
            , InsertPA_ByType mappingCategoryByType
            , string serviceName
            , bool isFromQueue)
        {
            var result = new InsertPAResultModel();

            if (problemAccounts.Any())
            {
                result = InsertProblemAccount(problemAccounts, paLosing, mappingCategoryByType, serviceName, isFromQueue);

                if (result.ErrorCode == 0)
                {
                    InsertPALogics(result.CustomerInfo);
                }
            }

            return result;
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
                        CategoryID = (int)CATEGORY.ROBOT,
                        IsFromTW = true,
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"])
                    });
                }
            }

            return Enumerable.Empty<InsertRobotModel>();
        }

        public IEnumerable<CustInfoModel> InsertRobotAccount(IEnumerable<InsertRobotModel> robotAccounts)
        {
            if (robotAccounts != null && robotAccounts.Any())
            {
                var customerInfo = InsertRobotAccount(JsonConvert.SerializeObject(robotAccounts));

                CheckNewPAAssociationCustomerForRollback(customerInfo);

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

        private InsertPAResultModel InsertProblemAccount(IEnumerable<InsertProblemAccountModel> problemAccounts
            , IEnumerable<ProblemAccountLosingClassificationModel> paLosing
            , InsertPA_ByType insertByType
            , string serviceName
            , bool isFromQueue)
        {
            var result = new InsertPAResultModel();

            if (problemAccounts != null && problemAccounts.Any())
            {
                var paClassification = MergeProblemAccountClassification(problemAccounts, paLosing);

                result = InsertProblemAccount(JsonConvert.SerializeObject(paClassification), insertByType, isFromQueue);

                if (result.ErrorCode != 0)
                {
                    var logMessage = new LogMessage(result.ErrorMessage).AddMessage(serviceName + "InsertProblemAccount").AddMessage("Data: " + JsonConvert.SerializeObject(paClassification)).ToString();
                    LogService.Error(logMessage);
                }
            }

            return result;
        }

        private void InsertPALogics(IEnumerable<CustInfoModel> customerInfo)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                CheckNewPAAssociationCustomerForRollback(customerInfo);

                var custIds = customerInfo.Select(x => x.CustID).Distinct();

                UpdateBetLimitOfFlaggedCustomer(custIds);
            }
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
                                                     IsFromCTS = group.pa.IsFromCTS,
                                                     IsFromTVS = group.pa.IsFromTVS,
                                                     IsFromTW = group.pa.IsFromTW,
                                                     IsDataChanged = group.pa.IsDataChanged,
                                                     ReasonID = group.pa.ReasonID,

                                                     BetCount = temp?.BetCount,
                                                     ActiveDays = temp?.ActiveDays,
                                                     TurnoverRM = temp?.TurnoverRM,
                                                     WinlossRM = temp?.WinlossRM,
                                                     WinlossStatus = temp?.WinlossStatus
                                                 });
            }

            return Enumerable.Empty<InsertProblemAccountModel>();
        }

        private InsertPAResultModel InsertProblemAccount(string problemAccountJson, InsertPA_ByType inputType, bool isFromQueue)
        {
            LogService.Info($"AddSystemFormulaBetAccount InsertProblemAccount PA json: { problemAccountJson}"); // for testing

            var criteria = new ProblemAccountMarkCriteria
            {
                ProblemAccountJson = problemAccountJson,
                IsFromQueue = isFromQueue,
                InputType = Enum.GetName(typeof(InsertPA_ByType), inputType)
            };

            var result = _repository.Fetch<CustInfoModel>(criteria);

            return new InsertPAResultModel
            {
                ErrorMessage = criteria.ErrorMessage,
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                CustomerInfo = result ?? Enumerable.Empty<CustInfoModel>()
            };
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

        private void CheckNewPAAssociationCustomerForRollback(IEnumerable<CustInfoModel> customerInfo)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(customerInfo);

                var criteria = new CheckNewPAAssociationCustomerForRollbackCriteria
                {
                    CustInfoJson = custInfoJson
                };

                _repository.Execute(criteria);
            }
        }

        private IEnumerable<CustInfoModel> InsertRobotAccount(string robotAccountJson)
        {
            var criteria = new RobotAccountInsertCriteria
            {
                CustInfo = robotAccountJson
            };

            return _repository.Fetch<CustInfoModel>(criteria) ?? Enumerable.Empty<CustInfoModel>();
        }

        #endregion Private Methods
    }
}