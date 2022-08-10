namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using System.Linq;

    public class DetectAssociationItemViewModel
    {              
        public DetectAssociationItemViewModel() { }

        public DetectAssociationItemViewModel(DetectAssociationMergeGroupModel data)
        {
            this.RowNumber = data.RowNumber;
            this.GroupID = data.GroupID;
            this.UserName = data.UserName;
            this.CTSCustID = data.CTSCustID;
            this.CustID = data.CustID;
            this.CategoryIDs = data.CategoryIDs;
            this.CustStatus = data.CustStatus;
            this.Evidences = data.Evidence;
            this.AssociatedTypes = data.AssociationType;
            this.CustomerCategory = data.CustomerCategory;
            this.CustomerClass = data.CustomerClass > 0 ? data.CustomerClass.ToString() : null;
            this.IsProblemAccount = data.ParentIDs != null && data.ParentIDs.Split(',').Any(y => y == "50");
            this.IsPAProbation = data.IsPAProbation;
        }

        public int RowNumber { get; set; }

        public int? GroupID { get; set; }

        public string UserName { get; set; }

        public string CTSCustIDList { get; set; }

        public long CTSCustID { get; set; }        

        public int CustID { get; set; }        

        public string CustStatus { get; set; }

        public string Evidences { get; set; }       

        public string AssociatedTypes { get; set; }

        public string CategoryIDs { get; set; }

        public string CustomerCategory { get; set; }

        public string CustomerClass { get; set; }

        public bool IsProblemAccount { get; set; }

        public bool IsPAProbation { get; set; }

    }
}
