namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System;

    public class TransactionParamModel
    {
        public int SubscriberID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public long? CTSCustID { get; set; }

        public string DeviceCode { get; set; }

        public string IP { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public string UserName { get; set; }

        public bool IsPartial { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}