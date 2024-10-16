namespace Fanex.NAP.CTS.Service.Implement.CustomerClassificationAgency
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
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

            InsertPALogicsAgency(insertPaResult.CustInfos);

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
                    var insertPaResult = InsertProblemAccountsAgency(problemAccounts, ccInputFlow, FunctionId.InsertPAM, isToQueue);

                    if (insertPaResult.CustInfos != null && insertPaResult.CustInfos.Any())
                    {
                        InsertPALogicsAgency(insertPaResult.CustInfos);

                        //_ccService.PushDataToExternalPackge(insertPaResult.CustInfos.Select(x => x.CustID), new PushDataToExternalModel
                        //{
                        //    ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                        //    IsUpdateDangerToMainDB = true,
                        //    IsSyncAFC = ConfigSettings.IsSyncAFCData
                        //}, fromFlow);
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

        public ResultRemoveCustomerClassificationAgencyModel UnmarkProblemAccountAgency(IEnumerable<UnmarkProblemAccountAgencyModel> paUnmark, bool isFromQueue)
        {
            var modelRemove = new RemoveCustomerClassificationAgencyModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(paUnmark),
                IsFromQueue = isFromQueue,
                FromAction = (int)FromFlow.CTSWEB
            };

            var removePaResult = base.RemoveCustomerClassificationAgency(AgencyCCInputFlow.RemovePACategory, modelRemove);

            RemovePALogics(removePaResult.CustInfos);

            return removePaResult;
        }

        #region private method
        private void RemovePALogics(IEnumerable<ResultRemoveCCInfoAgencyModel> customerInfo)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var custIds = customerInfo.Where(c => c.IsCleanPA).Select(x => x.CustID).Distinct();

                UpdateBetLimitOfUnflaggedCustomer(custIds);
            }
        }

        private void InsertPALogicsAgency(IEnumerable<CustInfoClassificationAgencyModel> customerInfo)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var custPas = customerInfo.Where(x => x.IsPA).Distinct();

                // not handle for SMA
                //CheckNewPAAssociationCustomerForRollback(JsonConvert.SerializeObject(custPas));

                UpdateBetLimitOfFlaggedCustomer(custPas.Select(x => x.CustID));
            }
        }

        private ResultInsertCustomerClassificationAgencyModel InsertProblemAccountsAgency(IEnumerable<InsertProblemAccountModel> problemAccounts, 
            AgencyCCInputFlow ccInputFlow, 
            FunctionId funcId, 
            bool isToQueue)
        {
            var paClassification = CombinePAWithLosingPerformace(problemAccounts, funcId);

            var modelInsert = new InsertCustomerClassificationAgencyModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(paClassification),
                IsToQueue = isToQueue,
                FromAction = (int)FromFlow.CTSWEB
            };

            return base.InsertCustomerClassificationAgency(ccInputFlow, modelInsert);
        }

        private IEnumerable<InsertProblemAccountAgencyModel> CombinePAWithLosingPerformace(IEnumerable<InsertProblemAccountModel> problemAccounts, FunctionId funcId)
        {
            return problemAccounts.Select(x => new InsertProblemAccountAgencyModel
            {

            });

            //return problemAccounts.Select(x => { var y = x;
            //    y.WinlossStatus = 2;
            //    y.TurnoverRM = 0;
            //    y.WinlossRM = 0;
            //    y.BetCount = 0;
            //    y.PerformanceTime = null;
            //    return y;
            //});
        }

        #endregion
    }
}
