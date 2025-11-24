namespace Fanex.NAP.CTS.Model.CustomerClassificationBySport.RescanCustomerClassificationBySport
{
    using System;

    public class RescanCustomerClassificationBySportResultModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int? CategoryID { get; set; }

        public int? SubscriberID { get; set;  }

        public bool? IsLicensee { get; set;  }

        public string Remark { get; set; }

        public Int64? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int SportID { get; set; }

        public int InputFlowID { get; set; }
    }
}
