namespace Fanex.NAP.CTS.ViewModel.DangerMonitor
{
    using System.Collections.Generic;

    public class GetDangerMonitorViewModel
    {
        public IEnumerable<DangerMonitorInfoViewModel> DangerMonitors { get; set; }

        public int LastID { get; set; }

        public string LastMatchSection { get; set; }
    }
}
