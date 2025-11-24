namespace Fanex.NAP.CTS.Model.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using System;

    public class CustomerClassificationBySportModel
    {
        public long CustID { get; set; }

        public int? CategoryID { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }

        public DateTime? PerformanceTime { get; set; }
    }
}
