namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence;
    using System.Collections.Generic;

    public interface ICustomerEvidenceService
    { 
        IEnumerable<CTSCustomerEvidenceViewModel> CheckCleanCustFlaggedEvidence(string ctsCustIds);

        void UpdateBetLimitOfFlaggedCustomer(string custIds);

        void UpdateBetLimitOfUnflaggedCustomer(string custIds);

        AddEvidenceResultModel AddCustEvidenceMultipleByCustId(string custIds, int evidenceId, int userId);
    }
}
