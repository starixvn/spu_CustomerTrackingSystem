namespace Fanex.NAP.CTS.Model.ProblemAccountManagement
{
    using System;

    public class GetProblemAccountModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public string Site { get; set; }

        public int SiteID { get; set; }

        public string Currency { get; set; }

        public int CurrencyID { get; set; }

        public string CategoryName { get; set; }

        public string CategoryID { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }
       
        public string Remark { get; set; }

        public int? TVSRequestID { get; set; }

        public bool IsFromTVS { get; set; }

        public bool? IsParlay { get; set; }

        public int? SportType { get; set; }

        public int IssueTypeID { get; set; }

    }
}
