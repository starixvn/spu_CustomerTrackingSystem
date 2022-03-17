namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IMonitorCtsFunctionService
    {
        DBResult StatsAccessmentInsert(SystemId systemId, TrackingCtsFunctionId funcId, int userId);
    }
}