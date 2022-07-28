namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    public class DetectAssociationItemViewModel
    {              
        public int RowNumber { get; set; }

        public int GroupID { get; set; }

        public string UserName { get; set; }

        public long CTSCustID { get; set; }

        public int CustID { get; set; }        

        public string CustStatus { get; set; }

        public string Evidences { get; set; }

        public string Categories { get; set; }

        public string AssociationTypes { get; set; }

        public string CategoryIDs { get; set; }

        public string CustomerCategory { get; set; }

        public int CustomerClass { get; set; }

        public bool IsProblemAccount { get; set; }

        public bool IsPAProbation { get; set; }

    }
}
