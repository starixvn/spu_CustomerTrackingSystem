namespace Fanex.NAP.CTS.Service.Implement.CustomerClassificationAgency
{
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.ProblemAccountAgency;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassificationAgency;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class ProblemAccountClassificationAgencyService : BaseCustomerClassificationAgencyService, IProblemAccountClassificationAgencyService
    {
        public ProblemAccountClassificationAgencyService() : base()
        {
        }

        public IEnumerable<long> RescanCustomerClassificationAgency(IEnumerable<RescanCustomerClassificationAgencyModel> custRescans, AgencyCCInputFlow ccInputFlow, bool isToQueue)
        {
            var pas = custRescans.Select(x => new InsertProblemAccountAgencyModel(x));

            var insertPaResult = InsertProblemAccountsAgency(pas, ccInputFlow, FunctionIdAgency.InsertPAM, isToQueue);

            InsertPALogicsAgency(insertPaResult.CustInfos);

            return insertPaResult.CustInfos.Select(x => x.CustID).Distinct();
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

        private ResultInsertCustomerClassificationAgencyModel InsertProblemAccountsAgency(IEnumerable<InsertProblemAccountAgencyModel> problemAccounts, AgencyCCInputFlow ccInputFlow, FunctionIdAgency funcId, bool isToQueue)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
