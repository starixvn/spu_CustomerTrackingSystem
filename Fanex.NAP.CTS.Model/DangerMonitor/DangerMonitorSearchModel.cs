namespace Fanex.NAP.CTS.Model.DangerMonitor
{
    using System;

    public class DangerMonitorSearchModel
    {
        public DangerMonitorSearchModel() { }

        public DangerMonitorSearchModel(DangerMonitorSearchModel model)
        {
            FromDate = model.FromDate;
            ToDate = model.ToDate;
            DangerList = model.DangerList;
            Sports = model.Sports;
            Event = model.Event;
            Market = model.Market;
            MatchSection = model.MatchSection;
            BetTypes = model.BetTypes;
        }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string DangerList { get; set; }

        public string Sports { get; set; }

        public string Event { get; set; } /* League / Match */

        public string Market { get; set; }

        public string MatchSection { get; set; }

        public string BetTypes { get; set; }

        public long? LastMatchID { get; set; }

        public string LastMatchSection { get; set; }

        public string LastEventDate { get; set; }
    }
}