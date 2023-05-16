namespace Fanex.NAP.CTS.Model.LatestTransaction
{
    using System;

    public class LatestTransactionModel
    {        
        public string UserName { get; set; }

        public string LoginID { get; set; }

        public long CTSCustID { get; set; }
      
        public DateTime TransTime { get; set; }        

        public string FirstDeviceCode { get; set; }

        public string IP { get; set; }

        public string Flagged { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string OS { get; set; }

        public string Browser { get; set; }

        public string URLDetails { get; set; }

        public long AccountID { get; set; }

        public long TransID { get; set; }

        public string SubscriberName { get; set; }

    }
}
