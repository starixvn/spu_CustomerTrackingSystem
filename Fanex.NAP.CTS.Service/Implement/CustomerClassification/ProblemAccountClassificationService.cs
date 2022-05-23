namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class ProblemAccountClassificationService : IProblemAccountClassificationService
    {
        private readonly IDynamicRepository _repository;
        private readonly ICustomerClassificationService _ccService;
        private readonly ICustomerEvidenceService _evidenceService;
        private readonly IRescanCustomerClassificationService _rescanCCService;
        private readonly IAntiFraudCenterService _antiFraudCenterService;
       
        public ProblemAccountClassificationService()
        {
            _repository = new DynamicRepository();
            _ccService = new CustomerClassificationService();
            _evidenceService = new CustomerEvidenceService();
            _rescanCCService = new RescanCustomerClassificationService();
            _antiFraudCenterService = new AntiFraudCenterService();
        }

        public bool InsertProblemAccountPackage(IEnumerable<InsertProblemAccountModel> problemAccounts, InsertPA_ByType mappingCategoryByType, string serviceName, FromFlow fromFlow)
        {
            try
            {
                if (problemAccounts.Any())
                {
                    var custIds = problemAccounts.Select(c => c.CustID)?.Distinct();

                    var probAccountLosing = GetProblemAccountLosingClassification(custIds, FunctionId.InsertPAM);

                    var result = InsertProblemAccount(problemAccounts, probAccountLosing, mappingCategoryByType, serviceName);

                    if (result.ErrorCode == 0)
                    {
                        InsertPALogics(result.CustomerInfo);

                        PushDataToExternal(result.CustomerInfo, fromFlow);
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                return false;
            }

            return true;
        }

        public InsertPAResultModel InsertProblemAccountLogics(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> paLosing, InsertPA_ByType mappingCategoryByType, string serviceName)
        {
            var result = new InsertPAResultModel();

            if (problemAccounts.Any())
            {
                result = InsertProblemAccount(problemAccounts, paLosing, mappingCategoryByType, serviceName);

                if (result.ErrorCode == 0)
                {
                    InsertPALogics(result.CustomerInfo);
                }
            }

            return result;
        }

        public IEnumerable<ExistedProblemAccountModel> GetExistedProblemAccounts(IEnumerable<long> custIds)
        {
            if(custIds != null && custIds.Any())
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

        #region Private Methods

        private InsertPAResultModel InsertProblemAccount(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> paLosing, InsertPA_ByType insertByType, string serviceName)
        {
            var result = new InsertPAResultModel();

            if (problemAccounts != null && problemAccounts.Any())
            {
                var paClassification = MergeProblemAccountClassification(problemAccounts, paLosing);

                result = InsertProblemAccount(JsonConvert.SerializeObject(paClassification), insertByType);

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

        private void PushDataToExternal(IEnumerable<CustInfoModel> customerInfo, FromFlow fromFlow)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var custIds = customerInfo.Select(x => x.CustID).Distinct();

                _ccService.PushDataToExternal(custIds, new PushDataToExternalModel
                {
                    ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                    IsPushCCToSoon88 = true,
                    IsUpdateDangerToMainDB = true,
                    DangerLevelType = DangerLevel.ABI
                });

                _antiFraudCenterService.InsertAFCAccounts(custIds, fromFlow);
            }
        }

        private IEnumerable<InsertProblemAccountModel> MergeProblemAccountClassification(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                return problemAccounts.GroupJoin(probAccountLosing,
                                            pa => pa.CustID,
                                            paLosing => paLosing.CustId,
                                            (pa, paLosing) => new { pa, paLosing })
                                            .SelectMany(
                                                 group => group.paLosing.DefaultIfEmpty(),
                                                 (group, temp) => new InsertProblemAccountModel
                                                 {
                                                     /* Cust info */
                                                     CustID = group.pa.CustID,
                                                     CTSCustID = group.pa.CTSCustID,
                                                     RoleID = group.pa.RoleID,
                                                     SubscriberID = group.pa.SubscriberID,
                                                     CategoryID = group.pa.CategoryID,
                                                     CategoryGroup = group.pa.CategoryGroup,
                                                     IsLicensee = group.pa.IsLicensee,
                                                     CreatedBy = group.pa.CreatedBy,
                                                     Remark = group.pa.Remark,

                                                     IsDirectPA = group.pa.IsDirectPA,
                                                     IsFromPAM = true,
                                                     ReasonID = group.pa.ReasonID,

                                                     /* Get Mapping Value */
                                                     BetCount = temp?.BetCount,
                                                     ActiveDays = temp?.ActiveDays,
                                                     TurnoverRM = temp?.TurnoverRM,
                                                     WinlossRM = temp?.WinlossRM,
                                                     WinlossStatus = temp?.WinlossStatus
                                                 });
            }

            return Enumerable.Empty<InsertProblemAccountModel>();
        }

        private InsertPAResultModel InsertProblemAccount(string problemAccountJson, InsertPA_ByType byType)
        {
            var criteria = new ProblemAccountInsertCriteria
            {
                ProblemAccountJson = problemAccountJson,
                ByType = Enum.GetName(typeof(InsertPA_ByType), byType)
            };

            var result = _repository.Fetch<CustInfoModel>(criteria);

            return new InsertPAResultModel
            {
                ErrorMessage = criteria.ErrorMessage,
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                CustomerInfo = result ?? Enumerable.Empty<CustInfoModel>()
            };
        }

        private void UpdateBetLimitOfFlaggedCustomer(IEnumerable<long> custId)
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

        private void UpdateBetLimitOfUnflaggedCustomer(IEnumerable<long> custId)
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


        #endregion
    }
}
