namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;

    public class MatchFromMainInfoModel
    {
        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }       

        public DateTime KickOffTime { get; set; }

        public int MatchSection { get; set; }

        public int IsGetDataFromOrigin { get; set; }

        public short DataSource { get; set; }
    }
}
