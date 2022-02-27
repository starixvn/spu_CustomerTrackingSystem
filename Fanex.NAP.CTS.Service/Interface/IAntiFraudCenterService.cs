namespace Fanex.NAP.CTS.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IAntiFraudCenterService
    {
        Task InsertAFCAccounts(IEnumerable<long> custIds, FromFlow fromFlow);

        Task DeleteAFCAccounts(IEnumerable<long> custIds, FromFlow fromFlow);
    }
}
