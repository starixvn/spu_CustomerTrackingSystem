namespace Fanex.NAP.CTS.Model.DangerMonitor
{
    using System;
    using System.Collections.Generic;

    public class DangerMonitorSearchModel
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string DangerList { get; set; }

        public string Sports { get; set; }

        public string Event { get; set; } /* League / Match */

        public string Market { get; set; }

        public string MatchSection { get; set; }

        public string BetTypes { get; set; }
    }
}