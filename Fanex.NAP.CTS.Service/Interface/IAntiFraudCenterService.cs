using System.Collections.Generic;

namespace Fanex.NAP.CTS.Service
{
    public interface IAntiFraudCenterService
    {
        void InsertAFCAccounts(IEnumerable<long> custIds, bool isSyncAFCData);

        void DeleteAFCAccounts(IEnumerable<long> custIds, bool isSyncAFCData);
    }
}
