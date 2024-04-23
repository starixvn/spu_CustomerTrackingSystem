namespace Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification
{
    using System;

    public class RescanInitialTWGroupBettingModel
    {
        public long CustID { get; set; }

        public int CTSCustID { get; set; }

        public int SubscriberID { get; set; }

        public int RoleID { get; set; }

        public int CategoryID { get; set; }

        public bool IsLicensee { get; set; }

        public bool IsFromAI { get; set; }

        public int CategoryGroup { get; set; }

        public DateTime DetectedDate { get; set; }

        public int TWGBTicketCount { get; set; }
    }
}
