namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    public class GetAssociationDetectionDetailViewModel
    {
        public long CTSCustID { get; set; }

        public string ListCTSCustID { get; set; }

        public bool IsBPManuallyDeviceIP { get; set; }

        public bool IsAgent { get; set; }

        public bool IsIPLast3Days { get; set; }

        public bool Is3MatchesLast7Days { get; set; }

    }
}
