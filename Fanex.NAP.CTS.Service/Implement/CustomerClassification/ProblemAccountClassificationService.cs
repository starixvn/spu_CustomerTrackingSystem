namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Interface;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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

        public bool InsertProblemAccount(IEnumerable<InsertProblemAccountModel> problemAccounts)
        {
            var custIdList = problemAccounts.Select(c => c.CustID)?.Distinct();
            var custIds = string.Format(",", custIdList);
            try
            {
                var probAccountLosing = _rescanCCService.GetProblemAccountLosing(custIdList, FunctionId.InsertPAM);

                var insertPACategoryResult = this.InsertProblemAccountByCategory(problemAccounts, probAccountLosing);

                var isSuccess = (insertPACategoryResult != null && insertPACategoryResult.ErrorCode == 0);

                if (isSuccess && insertPACategoryResult.CustIds != null && insertPACategoryResult.CustIds.Any())
                {
                    _ccService.PushDataToExternal(custIdList, new PushDataToExternalModel
                    {
                        ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                        IsPushCCToSoon88 = true,
                        IsUpdateDangerToMainDB = true,
                        DangerLevelType = DangerLevel.ABI
                    });

                    _evidenceService.UpdateBetLimitOfFlaggedCustomer(custIds);

                    _antiFraudCenterService.InsertAFCAccounts(custIdList, FromFlow.CTSServices);

                    _rescanCCService.CheckNewPAAssociationCustomerForRollback(insertPACategoryResult.NewPAAssociationCustomerForRollback);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
           
            return true;
        }

        public void InsertProblemAccountByReason(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing)
        {

        }

        public ProblemAccountInsertResultModel InsertProblemAccountByCategory(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing)
        {
            var proAccountClassification = MergeProblemAccountClassification(problemAccounts, probAccountLosing);

            if (proAccountClassification.Any())
            {
                var problemAccountJson = JsonConvert.SerializeObject(proAccountClassification);

                var criteria = new ProblemAccountInsertByCategoryCriteria
                {
                    ProblemAccountJson = problemAccountJson
                };

                var assCustForRollback = _repository.Fetch<CheckPACustomerAssociationForRollbackModel>(criteria);

                return new ProblemAccountInsertResultModel
                {
                    ErrorMessage = criteria.ErrorMessage,
                    ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                    CustIds = assCustForRollback?.Select(x => x.CustID) ?? Enumerable.Empty<long>(),
                    NewPAAssociationCustomerForRollback = assCustForRollback
                };
            }

            return new ProblemAccountInsertResultModel { CustIds = new List<long>(), ErrorCode = 0, ErrorMessage = string.Empty };
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
                                                     CustID = group.pa.CustID,
                                                     ReasonID = group.pa.ReasonID,
                                                     RobotCounter = group.pa.RobotCounter,
                                                     CreatedBy = group.pa.CreatedBy,
                                                     BetCount = temp?.BetCount,
                                                     ActiveDays = temp?.ActiveDays,
                                                     TurnoverRM = temp?.TurnoverRM,
                                                     WinlossRM = temp?.WinlossRM,
                                                     WinlossStatus = temp?.WinlossStatus
                                                 });
            }

            return Enumerable.Empty<InsertProblemAccountModel>();
        }
    }
}
