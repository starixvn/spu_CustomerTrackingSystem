namespace Fanex.NAP.CTS.Model.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using System;

    public class RescanCustomerClassificationBySportModel
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public Int64? CreatedBy { get; set; }

        public int SportGroup { get; set; }

        public int InputFlowID { get; set; }
    }
}
