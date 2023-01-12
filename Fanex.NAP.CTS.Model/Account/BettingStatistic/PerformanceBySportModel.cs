namespace Fanex.NAP.CTS.Model.Account
{
    public class PerformanceBySportModel
    { 
        public int SportID { get; set; }

        public string SportName { get; set; }

        public decimal TurnOver { get; set; }

        public decimal WinLoss { get; set; }

        public long BetCount { get; set; }

        public int Ranking { get; set; }
    }
}
