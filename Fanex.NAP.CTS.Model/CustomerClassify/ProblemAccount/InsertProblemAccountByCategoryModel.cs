namespace Fanex.NAP.CTS.Model.CustomerClassify
{
    using System;

    public class InsertProblemAccountByCategoryModel
    {
        public Int64 CustID { get; set; }

        public int CategoryID { get; set; }

        public int? WinlossStatus { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }

        public Int64? CreatedBy { get; set; }
    }
}