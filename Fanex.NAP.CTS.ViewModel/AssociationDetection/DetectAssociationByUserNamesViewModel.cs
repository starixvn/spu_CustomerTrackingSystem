namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{  
    public class DetectAssociationByUserNamesViewModel
    {
        public string UserNames { get; set; }

        public bool IsBPManuallyDeviceIP { get; set; }

        public bool IsAgent { get; set; }

        public bool IsIPLast3Days { get; set; }

        public bool Is3MatchesLast7Days { get; set; }

        public string AssociationByIPJson { get; set; }
    }
}
