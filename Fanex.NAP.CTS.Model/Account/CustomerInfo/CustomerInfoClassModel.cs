namespace Fanex.NAP.CTS.Model.Account.CustomerInfo
{
    using System;
   
    public class CustomerInfoClassModel
    {
        public long CustID { get; set; }

        public int CustomerClass { get; set; }

        public string CustomerClassName { get; set; }

        public int Category { get; set; }

        public DateTime ScannedTime { get; set; }
    }
}
