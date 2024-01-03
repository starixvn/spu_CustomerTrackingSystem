namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
    using System.Collections.Generic;

    public interface IDangerMonitorService
    {
        IEnumerable<DangerMonitorBetTypeViewModel> GetSportBettypes();
    }
}
