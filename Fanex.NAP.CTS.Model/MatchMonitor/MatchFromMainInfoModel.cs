namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;

    public class MatchFromMainInfoModel
    {
        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }       

        public DateTime KickOffTime { get; set; }

        public int MatchSection { get; set; }

        public bool IsGetDataFromOrigin { get; set; }

        public short DataSource { get; set; }

        public string LeagueName { get; set; }

        public string HomeName { get; set; }

        public string AwayName { get; set; }
    }
}
