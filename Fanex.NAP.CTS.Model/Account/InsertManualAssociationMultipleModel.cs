namespace Fanex.NAP.CTS.Model.Account
{
    public class InsertManualAssociationMultipleModel
    {
        public int FromSubscriberID { get; set; }

        public long FromCTSCustID { get; set; }

        public int ToSubscriberID { get; set; }

        public long ToCTSCustID { get; set; }

        public string Remark { get; set; }

        public int CreatedBy { get; set; }
    }
}
