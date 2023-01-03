namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class CustomerInfoByCTSCustIdModel
    {
        public int CustID { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public string SiteName { get; set; }

        public int CustSubID { get; set; }

        public DateTime LastLoginTime { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public DateTime CreatedDate { get; set; }

        public int RoleID { get; set; }

        public int? CustStatusID { get; set; }

        public string CustStatusName { get; set; }

        public bool IsLicensee { get; set; }

        public bool IsInternal { get; set; }

        public bool IsLicenseeVIP { get; set; }

        public bool IsLicenseeBA { get; set; }
    }
}
