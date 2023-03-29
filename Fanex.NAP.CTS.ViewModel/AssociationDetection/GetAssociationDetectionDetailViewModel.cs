namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    public class GetAssociationDetectionDetailViewModel
    {
        public string UserName { get; set; }

        public string UserNameList { get; set; }

        public int CustID { get; set; }        

        public long CTSCustID { get; set; }

        public string CustIDList { get; set; }

        public string CTSCustIDList { get; set; }

        public bool IsBP { get; set; }

        public bool IsDevice { get; set; }

        public bool IsIP { get; set; }

        public bool IsAgent { get; set; }

        public bool IsIPLast3Days { get; set; }

        public bool Is3MatchesLast7Days { get; set; }     
       
    }
}
