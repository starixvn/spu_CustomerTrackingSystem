namespace Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification
{
    using System;

    public class TWInitialSmartPAModel
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int SubscriberID { get; set; }

        public int RoleID { get; set; }

        public int CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsLicensee { get; set; }

        public DateTime DetectedDate { get; set; }

        public decimal Probability { get; set; }
    }
}
