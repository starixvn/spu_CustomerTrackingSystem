namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class CustomerClassificationHistoryModel
    {
        public DateTime CreatedTime { get; set; }

        public int ItemID { get; set; }

        public string SourceTypeName { get; set; }

        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string CustomerClass { get; set; }        

        public decimal TurnOver { get; set; }

        public decimal Winloss { get; set; }

        public decimal Margin { get; set; }

        public int BetCount { get; set; }

        public int ActiveDays { get; set; }

        public decimal LastXDaysMargin { get; set; }

        public decimal ProbationPeriodWinloss { get; set; }

        public string CreatedBy { get; set; }

    }
}
