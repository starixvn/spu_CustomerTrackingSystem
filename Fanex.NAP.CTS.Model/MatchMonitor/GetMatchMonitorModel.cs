namespace Fanex.NAP.CTS.Model.MatchMonitor
{   
    using System;

    public class GetMatchMonitorModel
    {
        public Int64 ID { get; set; }

        public Int64 MatchID { get; set; }

        public DateTime EventDate { get; set; }

        public int Market { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public int LeagueID { get; set; }

        public string LeagueName { get; set; }

        public int Sporttype { get; set; }

        public int BettypeID { get; set; }

        public int? BetID { get; set; }

        public string Reason { get; set; }        

        public int MatchSection { get; set; }

        public bool IsVerified { get; set; }

        public string VerifiedBy { get; set; }

        public DateTime KickOffTime { get; set; }

        public bool IsSupportShowAllTickets { get; set; }
    }
}
