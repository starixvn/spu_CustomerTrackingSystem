namespace Fanex.NAP.CTS.Model.Home
{
    using System;

    public class SearchAccountResultModel
    {
        public Int64 CTSCustId { get; set; }

        public int CustId { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public int RoleID { get; set; }

        public string RoleName { get; set; }

        public int CurrencyID { get; set; }

        public string Currency { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public DateTime LastLogin { get; set; }
    }
}
