namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class CustomerClassificationHistoryModel
    {
        public DateTime CreatedTime { get; set; }

        public int SourceTypeID { get; set; }

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

        public long TWBetCount { get; set; }

        public decimal TWGroupBettingRate { get; set; }

        public decimal TWTicketRejectRate { get; set; }

        public decimal TWDesktopUsageRate { get; set; }

        public int RobotCounter { get; set; }

        public int LastModifiedBy { get; set; }

        public string Remark { get; set; }

        public int TVSRequestID { get; set; }

        public bool IsFromTVS { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromCTS { get; set; }

    }
}
