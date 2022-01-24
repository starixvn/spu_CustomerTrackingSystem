namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;

    public class MatchMonitorFilterViewModel
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string Sports { get; set; }

        public string BetTypes { get; set; }

        public int? IsVerified { get; set; }

        public string LeagueName { get; set; }

        public int? Market { get; set; }

        public string MatchSection { get; set; }
    }
}
