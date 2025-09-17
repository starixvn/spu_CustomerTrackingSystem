namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.ProblemAccount;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Interface;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NAP.CTS.Core.Service;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class ProblemAccountClassificationService : BaseCustomerClassificationService, IProblemAccountClassificationService
    {
        private readonly ICriteriaService _repository;
        private readonly ICustomerClassificationService _ccService;
        private readonly IDataLogTrackingServices dataLogTrackingServices;

        public ProblemAccountClassificationService()
        {
            _repository = new CriteriaService();
            _ccService = new CustomerClassificationService();
            dataLogTrackingServices = new DataLogTrackingServices();
        }

        public IEnumerable<long> RescanCustomerClassification(IEnumerable<RescanCustomerClassificationModel> custRescans, CCInputFlow ccInputFlow, bool isToQueue)
        {
            var pas = custRescans.Select(x => new InsertProblemAccountModel(x));

            var insertPaResult = InsertProblemAccounts(pas, ccInputFlow, FunctionId.InsertPAM, isToQueue);

            InsertPALogics(insertPaResult.CustInfos);

            return insertPaResult.CustInfos.Select(x => x.CustID).Distinct();
        }

        public DBResult InsertProblemAccountPackage(IEnumerable<InsertProblemAccountModel> problemAccounts, CCInputFlow ccInputFlow, string serviceName, FromFlow fromFlow, bool isToQueue)
        {
            try
            {
                if (problemAccounts.Any())
                {
                    var insertPaResult = InsertProblemAccounts(problemAccounts, ccInputFlow, FunctionId.InsertPAM, isToQueue);

                    if (insertPaResult.CustInfos != null && insertPaResult.CustInfos.Any())
                    {
                        InsertPALogics(insertPaResult.CustInfos);

                        _ccService.PushDataToExternalPackge(insertPaResult.CustInfos.Select(x => x.CustID), new PushDataToExternalModel
                        {
                            RoleGroup = RoleGroupType.Member,
                            ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                            IsUpdateDangerToMainDB = true,
                            IsSyncAFC = ConfigSettings.IsSyncAFCData
                        }, fromFlow);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError();

                dataLogTrackingServices.LogErrorData(ex, serviceName);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        private ResultInsertCustomerClassificationModel InsertProblemAccounts(IEnumerable<InsertProblemAccountModel> problemAccounts, CCInputFlow ccInputFlow, FunctionId funcId, bool isToQueue)
        {
            var paClassification = CombinePAWithLosingPerformace(problemAccounts, funcId);

            var modelInsert = new InsertCustomerClassificationModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(paClassification),
                IsToQueue = isToQueue,
                FromAction = (int)FromFlow.CTSWEB
            };

            return base.InsertCustomerClassification(ccInputFlow, modelInsert);
        }

        private IEnumerable<InsertProblemAccountModel> CombinePAWithLosingPerformace(IEnumerable<InsertProblemAccountModel> problemAccounts, FunctionId funcId)
        {
            var custInfos = problemAccounts
                               .Select(c =>
                                   new PADailyScanFromQueueModel
                                   {
                                       CustID = c.CustID,
                                       DWSportType = c.DWSportType
                                   })
                               .Distinct(x => new { x.CustID, x.DWSportType });

            var paLosing = GetProblemAccountLosingClassification(custInfos, funcId);

            return MergeProblemAccountClassification(problemAccounts, paLosing);
        }

        public IEnumerable<ProblemAccountLosingClassificationModel> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId)
        {
            var custInfos = custIds?.Select(x => new PADailyScanFromQueueModel { CustID = x });

            return GetProblemAccountLosingClassification(custInfos, funcId);
        }

        public IEnumerable<ProblemAccountLosingClassificationModel> GetProblemAccountLosingClassification(IEnumerable<PADailyScanFromQueueModel> custInfos, FunctionId funcId)
        {
            if (custInfos.IsNotNullOrEmpty())
            {
                var criteria = new ProblemAccountLosingClassificationGetCriteria()
                {
                    CustJson = JsonConvert.SerializeObject(custInfos),
                    FunctionId = (int)funcId
                };

                return _repository.Fetch<ProblemAccountLosingClassificationModel>(criteria) ?? Enumerable.Empty<ProblemAccountLosingClassificationModel>();
            }

            return Enumerable.Empty<ProblemAccountLosingClassificationModel>();
        }

        public ResultRemoveCustomerClassificationModel UnmarkProblemAccount(IEnumerable<UnmarkProblemAccountModel> paUnmark, bool isFromQueue)
        {
            var modelRemove = new RemoveCustomerClassificationModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(paUnmark),
                IsFromQueue = isFromQueue,
                FromAction = (int)FromFlow.CTSWEB
            };

            var removePaResult = base.RemoveCustomerClassification(CCInputFlow.RemovePACategory, modelRemove);

            RemovePALogics(removePaResult.CustInfos);

            return removePaResult;
        }

        #region Private Methods

        private void InsertPALogics(IEnumerable<CustInfoClassificationModel> customerInfo)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var custPas = customerInfo.Where(x => x.IsPA).Distinct();

                CheckNewPAAssociationCustomerForRollback(JsonConvert.SerializeObject(custPas));

                UpdateBetLimitOfFlaggedCustomer(custPas.Select(x => x.CustID));
            }
        }

        private void RemovePALogics(IEnumerable<ResultRemoveCCInfoModel> customerInfo)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var custIds = customerInfo.Where(c => c.IsCleanPA).Select(x => x.CustID).Distinct();

                UpdateBetLimitOfUnflaggedCustomer(custIds);
            }
        }

        private IEnumerable<InsertProblemAccountModel> MergeProblemAccountClassification(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                return problemAccounts.GroupJoin(probAccountLosing ?? Enumerable.Empty<ProblemAccountLosingClassificationModel>(),
                                            pa => $"{pa.CustID}_{pa.DWSportType}",
                                            pl => $"{pl.CustId}_{pl.SportGroup}",
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
                                                     IsLicensee = group.pa.IsLicensee,
                                                     CreatedBy = group.pa.CreatedBy,
                                                     LastModifiedDate = group.pa.LastModifiedDate,
                                                     CreatedDate = group.pa.CreatedDate,
                                                     Remark = group.pa.Remark,

                                                     IsMarkedDirectly = group.pa.IsMarkedDirectly,
                                                     IsFromAI = group.pa.IsFromAI,
                                                     IsFromCTS = group.pa.IsFromCTS,
                                                     IsFromTVS = group.pa.IsFromTVS,
                                                     RobotCounter = group.pa.RobotCounter,
                                                     TVSRequestID = group.pa.TVSRequestID,
                                                     IsFromTW = group.pa.IsFromTW,
                                                     TVSReasonID = group.pa.TVSReasonID,
                                                     IsParlay = group.pa.IsParlay,
                                                     SportType = group.pa.SportType,
                                                     DWSportType = group.pa.DWSportType,
                                                     IssueTypeID = group.pa.IssueTypeID,

                                                     BetCount = temp?.BetCount,
                                                     ActiveDays = temp?.ActiveDays,
                                                     TurnoverRM = temp?.TurnoverRM,
                                                     WinlossRM = temp?.WinlossRM,
                                                     WinlossStatus = temp?.WinlossStatus,
                                                     PerformanceTime = temp?.PerformanceTime
                                                 });
            }

            return Enumerable.Empty<InsertProblemAccountModel>();
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

        #endregion Private Methods

    }
}