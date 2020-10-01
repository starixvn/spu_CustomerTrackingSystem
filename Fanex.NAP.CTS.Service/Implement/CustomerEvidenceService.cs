namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Newtonsoft.Json;
    using Fanex.NAP.CTS.Criteria.ImportEvidences;
    using Fanex.Data.Repository;
    using System.Linq;
    using System;

    public class CustomerEvidenceService : ICustomerEvidenceService
    {
        private IDynamicRepository _repository;
        public CustomerEvidenceService()
        {
            _repository = new DynamicRepository();
        }
        public IEnumerable<int> GetAssociatedAccounts(GetAssociatedAccountByCTSCustIdsCriteria criteria)
        {
            return DataService.GetData<int>(criteria);
        }

        public IEnumerable<int> GetRemoveEvidenceAssociatedAccounts(GetRemoveEvidenceAssociatedAccountCriteria criteria)
        {
            return DataService.GetData<int>(criteria);
        }

        public IEnumerable<int> GetCleanAccounts(GetCleanAccountByCTSCustIdsCriteria criteria)
        {
            return DataService.GetData<int>(criteria);
        }

        public void UpdateBetLimitOfFlaggedCustomer(UpdateBetLimitByEvidenceAddingCriteria criteria)
        {
            DataService.Execute(criteria);
        }

        public void UpdateBetLimitOfUnflaggedCustomer(UpdateBetLimitByEvidenceRemovingCriteria criteria)
        {
            DataService.Execute(criteria);
        }

        public string AddCustEvidenceMultipleByCustId(string custIds, int evidenceId, int userId)
        {
            var custEvidenceList = custIds.Split(',').Select(a => new AddAccountEvidenceModel()
            {
                CustID = Convert.ToInt32(a),
                EvidenceID = evidenceId
            });

            var jsonData = JsonConvert.SerializeObject(custEvidenceList);

            var criteria = new InsertMultipleCustEviByCustIdCriteria
            {
                NewAccountEvidences = jsonData,
                UserId = userId,
                ErrorMessage = string.Empty
            };

            _repository.Execute(criteria);

            return criteria.ErrorMessage;
        }
    }
}