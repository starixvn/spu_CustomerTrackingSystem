namespace Fanex.NAP.CTS.Model.Common
{
    using System;

    public class TicketDetailsModel
    {
        public long TransId { get; set; }

        public DateTime TransDate { get; set; }

        public long MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public string EventStatus { get; set; }

        public DateTime KickOffTime { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public int LeagueId { get; set; }

        public int BetType { get; set; }

        public string TypeName { get; set; }

        public long CustId { get; set; }

        public string UserName { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal StakeRM { get; set; }

        public decimal Odds { get; set; }

        public decimal Hdp1 { get; set; }

        public decimal Hdp2 { get; set; }

        public int? LiveHomeScore { get; set; }

        public int? LiveAwayScore { get; set; }

        public string TicketStatus { get; set; }

        public bool LiveIndicator { get; set; }

        public string IP { get; set; }
    }
}
