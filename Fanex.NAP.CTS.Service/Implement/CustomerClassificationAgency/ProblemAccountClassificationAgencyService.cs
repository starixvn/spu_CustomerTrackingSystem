namespace Fanex.NAP.CTS.Service.Implement.CustomerClassificationAgency
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Criteria.CustomerClassificationAgency.ProblemAccount;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.ProblemAccountAgency;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassificationAgency;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class ProblemAccountClassificationAgencyService : BaseCustomerClassificationAgencyService, IProblemAccountClassificationAgencyService
    {
        private readonly ICustomerClassificationService _ccService;
        private readonly IDataLogTrackingServices _dataLogTrackingServices;
        public ProblemAccountClassificationAgencyService() : base()
        {
            _ccService = new CustomerClassificationService();
            _dataLogTrackingServices = new DataLogTrackingServices();
        }

        public IEnumerable<long> RescanCustomerClassificationAgency(IEnumerable<RescanCustomerClassificationAgencyModel> custRescans, AgencyCCInputFlow ccInputFlow, bool isToQueue)
        {
            var pas = custRescans.Select(x => new InsertProblemAccountAgencyModel(x));

            var insertPaResult = InsertProblemAccountsAgency(pas, ccInputFlow, FunctionId.InsertPAM, isToQueue);

            //InsertPALogicsAgency(insertPaResult.CustInfos);

            return insertPaResult.CustInfos.Select(x => x.CustID).Distinct();
        }

        public DBResult InsertProblemAccountAgencyPackage(IEnumerable<InsertProblemAccountModel> problemAccounts,
            AgencyCCInputFlow ccInputFlow, 
            string serviceName, 
            FromFlow fromFlow, 
            bool isToQueue)
        {
            try
            {
                if (problemAccounts.Any())
                {
                    var agencyPAs = problemAccounts.Select(x => new InsertProblemAccountAgencyModel(x));

                    var insertPaResult = InsertProblemAccountsAgency(agencyPAs, ccInputFlow, FunctionId.InsertPAM, isToQueue);

                    if (insertPaResult.CustInfos != null && insertPaResult.CustInfos.Any())
                    {
                        //InsertPALogicsAgency(insertPaResult.CustInfos);

                        _ccService.PushDataToExternalPackge(insertPaResult.CustInfos.Select(x => x.CustID), new PushDataToExternalModel
                        {
                            RoleGroup = RoleGroupType.Agency,
                            ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                            IsUpdateDangerToMainDB = false, // Agency not update danger
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

                _dataLogTrackingServices.LogErrorData(ex, serviceName);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        //public void UpdateBetLimitOfFlaggedCustomer(IEnumerable<long> custId)
        //{
        //    if (custId != null && custId.Any())
        //    {
        //        var criteria = new BetLimitByEvidenceUpdateCriteria
        //        {
        //            CustIDList = string.Join(",", custId)
        //        };

        //        _repository.Execute(criteria);
        //    }
        //}

        //public void UpdateBetLimitOfUnflaggedCustomer(IEnumerable<long> custId)
        //{
        //    if (custId != null && custId.Any())
        //    {
        //        var criteria = new BetLimitByEvidenceRemoveCriteria
        //        {
        //            CustIDList = string.Join(",", custId)
        //        };

        //        _repository.Execute(criteria);
        //    }
        //}

        public ResultRemoveCustomerClassificationAgencyModel UnmarkProblemAccountAgency(IEnumerable<UnmarkProblemAccountAgencyModel> paUnmark, bool isFromQueue)
        {
            var modelRemove = new RemoveCustomerClassificationAgencyModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(paUnmark),
                IsFromQueue = isFromQueue,
                FromAction = (int)FromFlow.CTSWEB
            };

            var removePaResult = base.RemoveCustomerClassificationAgency(AgencyCCInputFlow.RemovePACategory, modelRemove);

            //RemovePALogics(removePaResult.CustInfos);

            return removePaResult;
        }

        #region private method
        //private void RemovePALogics(IEnumerable<ResultRemoveCCInfoAgencyModel> customerInfo)
        //{
        //    if (customerInfo != null && customerInfo.Any())
        //    {
        //        var custIds = customerInfo.Where(c => c.IsCleanPA).Select(x => x.CustID).Distinct();

        //        UpdateBetLimitOfUnflaggedCustomer(custIds);
        //    }
        //}

        //private void InsertPALogicsAgency(IEnumerable<CustInfoClassificationAgencyModel> customerInfo)
        //{
        //    if (customerInfo != null && customerInfo.Any())
        //    {
        //        var custPas = customerInfo.Where(x => x.IsPA).Distinct();

        //        // not handle for SMA
        //        //CheckNewPAAssociationCustomerForRollback(JsonConvert.SerializeObject(custPas));

        //        UpdateBetLimitOfFlaggedCustomer(custPas.Select(x => x.CustID));
        //    }
        //}

        private ResultInsertCustomerClassificationAgencyModel InsertProblemAccountsAgency(IEnumerable<InsertProblemAccountAgencyModel> problemAccounts, 
            AgencyCCInputFlow ccInputFlow, 
            FunctionId funcId, 
            bool isToQueue)
        {
            var paClassification = CombinePAWithLosingPerformace(problemAccounts);

            var modelInsert = new InsertCustomerClassificationAgencyModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(paClassification),
                IsToQueue = isToQueue,
                FromAction = (int)FromFlow.CTSWEB
            };

            return base.InsertCustomerClassificationAgency(ccInputFlow, modelInsert);
        }

        private IEnumerable<InsertProblemAccountAgencyModel> CombinePAWithLosingPerformace(IEnumerable<InsertProblemAccountAgencyModel> problemAccounts)
        {          
            var custIDs = problemAccounts.Select(x => x.CustID)?.Distinct();

            var paLosing = GetProblemAccountLosingAgencies(custIDs);

            return MergePALosingClassification(problemAccounts, paLosing);
        }

        public IEnumerable<ProblemAccountLosingAgencyModel> GetProblemAccountLosingAgencies(IEnumerable<long> custIDs)
        {
            if (custIDs != null && custIDs.Any())
            {
                var criteria = new ProblemAccountLosingAgencyGetCriteria()
                {
                    CustIDList = string.Join(",", custIDs)
                };

                return _repository.Fetch<ProblemAccountLosingAgencyModel>(criteria);
            }

            return Enumerable.Empty<ProblemAccountLosingAgencyModel>();
        }

        private IEnumerable<InsertProblemAccountAgencyModel> MergePALosingClassification(IEnumerable<InsertProblemAccountAgencyModel> problemAccounts, IEnumerable<ProblemAccountLosingAgencyModel> paLosing)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                return problemAccounts.GroupJoin(paLosing ?? Enumerable.Empty<ProblemAccountLosingAgencyModel>(),
                                            pa => pa.CustID,
                                            pl => pl.CustId,
                                            (pa, pl) => new { pa, pl })
                                            .SelectMany(
                                                 group => group.pl.DefaultIfEmpty(),
                                                 (group, temp) => new InsertProblemAccountAgencyModel
                                                 {
                                                     CustID = group.pa.CustID,
                                                     CTSCustID = group.pa.CTSCustID,
                                                     RoleID = group.pa.RoleID,
                                                     SubscriberID = group.pa.SubscriberID,
                                                     CategoryID = group.pa.CategoryID,
                                                     IsLicensee = group.pa.IsLicensee,
                                                     CreatedBy = group.pa.CreatedBy,
                                                     //LastModifiedDate = group.pa.LastModifiedDate,
                                                     CreatedDate = group.pa.CreatedDate,
                                                     Remark = group.pa.Remark,

                                                     IsMarkedDirectly = group.pa.IsMarkedDirectly,
                                                     IsFromAI = group.pa.IsFromAI,
                                                     IsFromCTS = group.pa.IsFromCTS,
                                                     //IsFromTVS = group.pa.IsFromTVS,
                                                     RobotCounter = group.pa.RobotCounter,
                                                     //TVSRequestID = group.pa.TVSRequestID,
                                                     IsFromTW = group.pa.IsFromTW,
                                                     //TVSReasonID = group.pa.TVSReasonID,
                                                     //IsParlay = group.pa.IsParlay,
                                                     //SportType = group.pa.SportType,
                                                     //IssueTypeID = group.pa.IssueTypeID,

                                                     BetCount = temp?.BetCount,
                                                     //ActiveDays = temp?.ActiveDays,
                                                     TurnoverRM = temp?.TurnoverRM,
                                                     WinlossRM = temp?.WinlossRM,
                                                     WinlossStatus = temp?.WinlossStatus,
                                                     PerformanceTime = temp?.PerformanceTime
                                                 });
            }

            return Enumerable.Empty<InsertProblemAccountAgencyModel>();
        }

        #endregion
    }
}
