namespace Fanex.NAP.CTS.Model.Account
{
    public class PerformanceByBettypeModel
    {
        public int BettypeId { get; set; }

        public string BettypeName { get; set; } 

        public int Ranking { get; set; }

        public bool LiveIndicator { get; set; }

        public decimal Turnover { get; set; }

        public decimal Winloss { get; set; }

        public int BetCount { get; set; }
    }
}
