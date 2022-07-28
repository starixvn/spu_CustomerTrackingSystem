namespace Fanex.NAP.CTS.Model.AssociationDetection
{   
    public class SharedIPMatchesModel
    {
        public int FromCustID { get; set; }

        public long FromCTSCustID { get; set; }

        public int ToCustID { get; set; }

        public long ToCTSCustID { get; set; }
    }
}
