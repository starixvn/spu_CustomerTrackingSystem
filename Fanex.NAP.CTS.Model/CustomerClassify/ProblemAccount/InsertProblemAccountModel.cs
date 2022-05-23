namespace Fanex.NAP.CTS.Model.CustomerClassify
{
    using System;

    public class InsertProblemAccountModel
    {
        /* Cust info */
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool IsLicensee { get; set; }

        public string Remark { get; set; }

        public Int64? CreatedBy { get; set; }

        /* Data warehouse, mainDB  info */

        public int? WinlossStatus { get; set; } // LOSING  = 0, KEEPSTATE = 1, WINNING = 2

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }

        /* Get Mapping Value */

        public int? CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public int? ReasonID { get; set; }

        public bool IsDirectPA { get; set; }

        public bool IsFromTVS { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromPAM { get; set; }

        public bool IsDataChanged { get; set; }              
    }
}