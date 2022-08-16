namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;

    public class TicketDetailModel
    {
        public long SequenceId { get; set; }

        public Int64 TransId { get; set; }

        public DateTime TransDate { get; set; }

        public Int64 MatchId { get; set; }

        public Int64 HomeId { get; set; }

        public Int64 AwayId { get; set; }

        public Int64 LeagueId { get; set; }

        public string LeagueName { get; set; }

        public DateTime EventDate { get; set; }

        public string EventStatus { get; set; }

        public DateTime KickOffTime { get; set; }

        public int BetType { get; set; }

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

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }
    }
}