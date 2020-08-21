namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class CustomerInfoByCTSCustIdModel
    {
        public DateTime LastLoginTime { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public int CustId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int SubscriberId { get; set; }

        public string SubscriberName { get; set; }

        public int RoleId { get; set; }

        public int CustSubId { get; set; }

    }
}
