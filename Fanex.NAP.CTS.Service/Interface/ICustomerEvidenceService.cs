namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using System.Collections.Generic;

    public interface ICustomerEvidenceService
    {
        IEnumerable<int> GetAssociatedAccounts(GetAssociatedAccountByCTSCustIdsCriteria criteria);

        IEnumerable<int> GetRemoveEvidenceAssociatedAccounts(GetRemoveEvidenceAssociatedAccountCriteria criteria);

        IEnumerable<int> GetCleanAccounts(GetCleanAccountByCTSCustIdsCriteria criteria);

        void UpdateBetLimitOfFlaggedCustomer(UpdateBetLimitByEvidenceAddingCriteria criteria);

        void UpdateBetLimitOfUnflaggedCustomer(UpdateBetLimitByEvidenceRemovingCriteria criteria);

        string AddCustEvidenceMultipleByCustId(string custIds, int evidenceId, int userId);

    }
}
