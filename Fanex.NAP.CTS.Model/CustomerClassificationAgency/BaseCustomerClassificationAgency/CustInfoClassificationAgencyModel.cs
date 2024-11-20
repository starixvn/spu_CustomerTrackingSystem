namespace Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency
{
    using System;

    public class CustInfoClassificationAgencyModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int CustomerClass { get; set; }

        public int SportID { get; set; }

        public int CategoryID { get; set; }

        public DateTime ScannedTime { get; set; }

        public bool IsPA { get; set; }
    }
}
