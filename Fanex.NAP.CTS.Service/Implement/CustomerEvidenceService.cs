namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Model.Account;

    public class CustomerEvidenceService : ICustomerEvidenceService
    {
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
    }
}