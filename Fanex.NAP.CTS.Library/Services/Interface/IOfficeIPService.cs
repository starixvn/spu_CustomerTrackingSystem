namespace Fanex.NAP.CTS.Library.Services.Interface
{
    using Fanex.NAP.CTS.Library.Models;
    using Fanex.NAP.CTS.Library.Models.FraudCase.ScannedOfficeIP;
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