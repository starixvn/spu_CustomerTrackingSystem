namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System;

    public class CustInfoClassificationModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int CustomerClass { get; set; }

        public int SportID { get; set; }

        public int CategoryID { get; set; }

        public DateTime ScannedTime { get; set; }

        public int CategoryType { get; set; }

    }
}
