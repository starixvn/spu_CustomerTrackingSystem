namespace Fanex.NAP.CTS.Model.AssociationDetection
{   
    public class SharedIPMatchesInfoModel
    {
        public int FromCustID { get; set; }

        public long FromCTSCustID { get; set; }

        public string FromUserName { get; set; }

        public int ToCustID { get; set; }

        public long ToCTSCustID { get; set; }

        public string ToUserName { get; set; }
    }
}
