namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.ManageSubscriber;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System.Collections.Generic;

    public interface IMatchMonitorService
    {
        List<MatchMonitorModel> GetMatchMonitor(MatchMonitorFilterViewModel filter);

    }
}