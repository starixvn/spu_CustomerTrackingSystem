namespace Fanex.NAP.CTS.Model.DangerMonitor
{
    using System;

    public class DangerMonitorSearchModel
    {
        public DangerMonitorSearchModel() { }

        public DangerMonitorSearchModel(DangerMonitorSearchModel model)
        {
            DangerType = model.DangerType;
            DangerList = model.DangerList;
            Sports = model.Sports;
            Event = model.Event;
            Market = model.Market;
            MatchSection = model.MatchSection;
            BetTypes = model.BetTypes;
            SabaLeagueGroup = model.SabaLeagueGroup;
            ExcludeLeagueGroup = model.ExcludeLeagueGroup;
        }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int DangerType { get; set; }

        public string DangerList { get; set; }

        public string Sports { get; set; }

        public string Event { get; set; } /* League / Match */

        public string Market { get; set; }

        public string MatchSection { get; set; }

        public string BetTypes { get; set; }

        public int DBSource { get; set; }
        
        public int? SabaLeagueGroup { get; set; }

        public string ExcludeLeagueGroup { get; set; }
    }
}