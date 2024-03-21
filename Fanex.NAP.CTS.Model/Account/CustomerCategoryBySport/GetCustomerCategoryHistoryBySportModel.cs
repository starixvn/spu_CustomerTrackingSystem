namespace Fanex.NAP.CTS.Model.Account.CustomerCategoryBySport
{
    using System;

    public class GetCustomerCategoryHistoryBySportModel
    {
        public long ID { get; set; }

        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int SportID { get; set; }

        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public long BetCount { get; set; }

        public int ActiveDays { get; set; }

        public int TargetCC { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string Remark { get; set; }

        public int SourceTypeID { get; set; }

        public string SourceTypeName { get; set; }
    }
}
