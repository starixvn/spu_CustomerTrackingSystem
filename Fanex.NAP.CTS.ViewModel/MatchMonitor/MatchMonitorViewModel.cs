using Fanex.NAP.CTS.Model.MatchMonitor;

namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    public class MatchMonitorViewModel
    {
        public bool IsHostPermission { get; set; }

        public long CustId { get; set; }

        public MatchMonitorStaticListModel MMStatic { get; set; }
    }
}