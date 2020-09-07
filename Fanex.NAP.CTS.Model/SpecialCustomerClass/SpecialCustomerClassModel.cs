namespace Fanex.NAP.CTS.Model.SpecialCustomerClass
{
    using System;

    public class SpecialCustomerClassModel
    {
        public long CTSCustID { get; set; }

        public string Username { get; set; }

        public long RootCTSCustID { get; set; }

        public string SubscriberName { get; set; }

        public int CustomerClass { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
