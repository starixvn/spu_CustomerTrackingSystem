namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;

    public class TicketDetailModel
    {
        public Int64 TransId { get; set; }

        public DateTime TransDate { get; set; }

        public Int64 MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public int BetType { get; set; }

        public Int64 HomeId { get; set; }

        public Int64 AwayId { get; set; }

        public Int64 LeagueId { get; set; }

        public Int64 CustId { get; set; }

        public string BetTeam { get; set; }

        public Decimal Stake { get; set; }

        public Decimal Odds { get; set; }

        public Decimal Hdp1 { get; set; }

        public Decimal Hdp2 { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public bool LiveIndicator { get; set; }

        public int TicketStatus { get; set; }
    }
}