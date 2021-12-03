using System.Collections.Generic;

namespace Fanex.NAP.CTS.Service
{
    public interface IAntiFraudCenterService
    {
        void InsertAFCAccounts(IEnumerable<long> custIds);

        void DeleteAFCAccounts(IEnumerable<long> custIds);
    }
}
