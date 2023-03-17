namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;

    public class MatchMonitorFilterViewModel
    {
        public int DateType { get; set; } /* #1: InsertDate, 2: EventDate */

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string Sports { get; set; }

        public int? Market { get; set; }

        public string MatchSection { get; set; }

        public int? IsVerified { get; set; }

        public string BetTypes { get; set; }

        public string  Reasons { get; set; }

        public string Event { get; set; } /* League / Match */

    }
}
