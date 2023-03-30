namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    public class AssociationCustByIPViewModel
    {      
        public long O { get; set; } // FromCTSCustID

        public int F1 { get; set; } // FromCustID

        public string X { get; set; } // FromUserName

        public long D { get; set; } // ToCTSCustID

        public int T1 { get; set; } // ToCustID

        public string Y { get; set; } // ToUserName

        public string IP { get; set; } // IP
    }
}
