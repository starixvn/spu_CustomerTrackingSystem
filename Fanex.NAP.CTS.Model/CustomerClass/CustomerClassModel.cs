using System;

namespace Fanex.NAP.CTS.Model.CustomerClass
{
    public class CustomerClassModel
    {
        public long CTSCustID { get; set; }

        public string Username { get; set; }

        public long RootCTSCustID { get; set; }

        public string SubscriberName { get; set; }

        public int CustomerClass { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedDateString { 
            get 
            {
                return CreatedDate.ToString("MMM dd, yyyy");
            } 
        }

    }
}
