namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    public class DetectAssociationMergeGroupModel
    {
        public int RowNumber { get; set; }

        public int ? GroupID { get; set; }

        public long CTSCustID { get; set; }

        public int CustID { get; set; }

        public string UserName { get; set; }

        public string CustStatus { get; set; }

        public string Evidence { get; set; }

        public string CustomerCategory { get; set; }

        public string AssociationType { get; set; }

        public int CustomerClass { get; set; }

        public string CategoryIDs { get; set; }

        public string ParentIDs { get; set; }

        public bool IsPAProbation { get; set; }
    }
}
