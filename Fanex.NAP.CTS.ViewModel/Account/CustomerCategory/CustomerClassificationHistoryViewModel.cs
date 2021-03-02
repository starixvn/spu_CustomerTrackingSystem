namespace Fanex.NAP.CTS.ViewModel.Account
{    
    public class CustomerClassificationHistoryViewModel
    {
        public string CreatedTimeString { get; set; }

        public string SourceTypeName { get; set; }

        public string CategoryName { get; set; }

        public string CustomerClass { get; set; }
        
        public int CategoryID { get; set; }

        public string Reason { get; set; }

        public decimal TurnOver { get; set; }

        public decimal Winloss { get; set; }

        public decimal Margin { get; set; }

        public decimal BetCount { get; set; }

        public int ActiveDays { get; set; }

        public decimal LastXDaysMargin { get; set; }

        public decimal ProbationPeriodWinloss { get; set; }

        public string CreatedBy { get; set; }

        public string TaggingType { get; set; }
    }
}
