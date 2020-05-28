namespace Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor
{
    public class DetailTicket
    {
        public string Username { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public int BettypeId { get; set; }

        public string BettypeName { get; set; }

        public long BetId { get; set; }

        public string BetDate { get; set; }

        public string Choice { get; set; }

        public decimal Handicap1 { get; set; }

        public decimal Handicap2 { get; set; }

        public string HtmlChoice { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public bool LiveIndicator { get; set; }

        public string LiveBetScore { get; set; }

        public decimal Odds { get; set; }

        public int OddsType { get; set; }

        public decimal Stake { get; set; }

        public decimal Winlost { get; set; }

        public string TicketStatus { get; set; }

        public string IP { get; set; }

        public string CurrencyName { get; set; }

        public int HighlightStatus { get; set; }

        public bool IsVoid { get; set; }

    }
}