namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    public class GetAssociationDetectionDetailViewModel
    {
        public string UserName { get; set; }

        public int CustID { get; set; }

        public long CTSCustID { get; set; }

        public string CTSCustIDList { get; set; }

        public bool IsBPManuallyDeviceIP { get; set; }

        public bool IsAgent { get; set; }

        public bool IsIPLast3Days { get; set; }

        public bool Is3MatchesLast7Days { get; set; }

        public string SharedIPJson { get; set; }

        public string SharedMatchesJson { get; set; }
    }
}
