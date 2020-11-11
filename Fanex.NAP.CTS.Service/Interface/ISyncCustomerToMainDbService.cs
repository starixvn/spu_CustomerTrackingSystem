namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using System.Collections.Generic;

    public interface ISyncCustomerToMainDbService
    {
        DBResult SyncCustomerClassToMainDb(IEnumerable<CustomerClassForMainDb> customerClassList, string syncAction);

        IEnumerable<EvidenceCategoryToCustomerClassForMainDb> GetLatestEvidenceCategory(string ctsCustIDs, bool isCustId, bool isExcludedDetailedCategory);

        DBResult SyncCustomerClassToMainDb(string ctsCustID, bool isCustId, bool isExcludedDetailedCategory);
    }
}
