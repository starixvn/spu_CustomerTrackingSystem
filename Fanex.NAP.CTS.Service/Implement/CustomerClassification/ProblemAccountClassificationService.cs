namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Model;
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

        public DBResult InsertProblemAccount(IEnumerable<InsertProblemAccountModel> problemAccounts, MappingCategoryByType mappingCategoryByType)
        {
            var custIdList = problemAccounts.Select(c => c.CustID)?.Distinct();
            var custIds = string.Join(",", custIdList); 
            try
            {
                var probAccountLosing = _rescanCCService.GetProblemAccountLosing(custIdList, FunctionId.InsertPAM);

                var insertPACategoryResult = this.InsertProblemAccount(problemAccounts, probAccountLosing, mappingCategoryByType);

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
                var logMessage = new LogMessage(ex.Message)
                                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
           
            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public ProblemAccountInsertResultModel InsertProblemAccount(IEnumerable<InsertProblemAccountModel> problemAccounts
            , IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing
            , MappingCategoryByType mappingCategoryByType)
        {
            var proAccountClassification = MergeProblemAccountClassification(problemAccounts, probAccountLosing);

            if (proAccountClassification.Any())
            {
                var problemAccountJson = JsonConvert.SerializeObject(proAccountClassification);

                var criteria = new ProblemAccountInsertCriteria
                {
                    ProblemAccountJson = problemAccountJson,
                    ByType = Enum.GetName(typeof(MappingCategoryByType), mappingCategoryByType)
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
                                                     RobotCounter = group.pa.RobotCounter,

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
    }
}
