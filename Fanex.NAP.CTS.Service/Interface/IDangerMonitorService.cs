namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
    using System.Collections.Generic;

    public interface IDangerMonitorService
    {
        IEnumerable<DangerMonitorBetTypeViewModel> GetSportBettypes();

        IEnumerable<DangerMonitorInfoViewModel> GetDangerMonitor(DangerMonitorSearchModel filter);
    }
}
