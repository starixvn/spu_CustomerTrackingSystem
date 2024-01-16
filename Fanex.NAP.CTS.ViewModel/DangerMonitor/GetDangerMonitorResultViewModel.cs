namespace Fanex.NAP.CTS.ViewModel.DangerMonitor
{
    using System;
    using System.Collections.Generic;

    public class GetDangerMonitorResultViewModel
    {
        public IEnumerable<GetDangerMonitorItemViewModel> Items { get; set; }

        public long? LastMatchID { get; set; }

        public string LastMatchSection { get; set; }

        public DateTime? LastEventDate { get; set; }
    }
}
