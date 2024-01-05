namespace Fanex.NAP.CTS.ViewModel.DangerMonitor
{
    public class DangerMonitorTicketSummaryViewModel
    {
        public long MatchId { get; set; }

        public int LiveIndicator { get; set; }

        public int Bettype { get; set; }

        public int BetID { get; set; }

        public int BetChoiceType { get; set; }

        public long TotalAmountHome { get; set; }

        public long TotalAmountAway { get; set; }

        public int TotalTicketHome { get; set; }

        public int TotalTicketAway { get; set; }
    }
}