namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.FraudCase.ScannedOfficeIP;
    using System.Collections.Generic;

    public interface IOfficeIPService
    {
        IEnumerable<OfficeIp> GetIps();

        DBResult AddIpRange(string fromIp, string toIp, int userId);

        DBResult DeleteIpRange(string fromIp, string toIp, int userId);

        IEnumerable<OfficeIPInternalAccount> GetInternalAccounts();

        DBResult AddInternalAccount(string userName, int userId);

        DBResult DeleteInternalAccount(int custId, int userId);
    }
}