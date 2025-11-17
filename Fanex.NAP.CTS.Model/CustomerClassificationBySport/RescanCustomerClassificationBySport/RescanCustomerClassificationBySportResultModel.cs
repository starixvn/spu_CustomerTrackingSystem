namespace Fanex.NAP.CTS.Model.CustomerClassificationBySport.RescanCustomerClassificationBySport
{
    using System;

    public class RescanCustomerClassificationBySportResultModel
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public Int64? CreatedBy { get; set; }

        public int SportID { get; set; }

        public int InputFlowID { get; set; }
    }
}
