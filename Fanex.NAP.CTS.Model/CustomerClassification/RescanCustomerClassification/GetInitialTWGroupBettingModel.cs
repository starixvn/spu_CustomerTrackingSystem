namespace Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification
{
    using System;

    public class GetInitialTWGroupBettingModel
    {
        public int CustID { get; set; }     

        public int? TWGBTicketCount { get; set; }

        public DateTime DetectedDate { get; set; }
    }
}
