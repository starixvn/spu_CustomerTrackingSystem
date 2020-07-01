namespace Fanex.NAP.CTS.Model.ProblemAccountMonitor
{    
    using System;

    public class DetailTicketsModel
    {
        public int  CustID { get; set; }

        public string Username { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public int BettypeId { get; set; }

        public string BettypeName { get; set; }

        public long TransId { get; set; }

        public DateTime TransDate { get; set; }

        public string BetTeam { get; set; }

        public decimal Handicap1 { get; set; }

        public decimal Handicap2 { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public bool LiveIndicator { get; set; }

        public decimal Odds { get; set; }

        public int OddsType { get; set; }

        public decimal Stake { get; set; }

        public decimal Winlost { get; set; }

        public string TicketStatus { get; set; }

        public int TicketStatusId { get; set; }

        public string IP { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public int HighlightStatus { get; set; }

        public int? GroupID { get; set; }
    }
}