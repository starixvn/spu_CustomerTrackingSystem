namespace Fanex.NAP.CTS.ViewModel.DangerMonitor
{
    using System;
    using System.Collections.Generic;

    public class GetDangerSummaryResultViewModel
    {
        public IEnumerable<GetDangerSummaryItemViewModel> Items { get; set; }

        public long? LastMatchId { get; set; }

        public string LastEventDate { get; set; }
    }
}
