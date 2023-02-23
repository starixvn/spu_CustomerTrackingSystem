namespace Fanex.NAP.CTS.Model.Transaction
{
    using System;

    public class TicketDetailsModel
    {
        public long TransId { get; set; }

        public DateTime TransDate { get; set; }

        public long MatchID { get; set; }

        public DateTime EventDate { get; set; }

        public string EventStatus { get; set; }

        public DateTime KickOffTime { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }

        public int LeagueID { get; set; }

        public string BetType { get; set; }

        public string TypeName { get; set; }

        public long CustID { get; set; }

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

        public string CurrencyName { get; set; }

    }
}
