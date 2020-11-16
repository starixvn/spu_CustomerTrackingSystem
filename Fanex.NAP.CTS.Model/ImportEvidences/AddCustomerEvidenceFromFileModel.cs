namespace Fanex.NAP.CTS.Model.ImportEvidences
{
    public class AddCustomerEvidenceFromFileModel
    {
        public long CTSCustID { get; set; }

        public int CustID { get; set; }

        public int SubscriberID { get; set; }

        public int EvidenceID { get; set; }

        public string Remark { get; set; }
    }
}
