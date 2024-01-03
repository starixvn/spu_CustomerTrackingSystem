namespace Fanex.NAP.CTS.Model.DangerMonitor
{
    using System;
    using System.Collections.Generic;

    public class DangerMonitorSearchModel
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public List<int> Danger1List { get; set; }

        public List<int> Danger2List { get; set; }

        public List<int> Danger3List { get; set; }

        public List<int> Danger4List { get; set; }

        public List<int> Danger5List { get; set; }

        public string Sports { get; set; }

        public string Event { get; set; } /* League / Match */

        public int? Market { get; set; }

        public string MatchSection { get; set; }

        public string BetTypes { get; set; }
    }
}