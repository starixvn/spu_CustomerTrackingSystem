namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using System.Collections.Generic;

    public interface ICustomerEvidenceService
    { 
        IEnumerable<int> CheckCleanCustFlaggedEvidence(string ctsCustIds);

        void UpdateBetLimitOfFlaggedCustomer(string custIds);

        void UpdateBetLimitOfUnflaggedCustomer(string custIds);

        AddEvidenceResultModel AddCustEvidenceMultipleByCustId(string custIds, int evidenceId, int userId);
    }
}
