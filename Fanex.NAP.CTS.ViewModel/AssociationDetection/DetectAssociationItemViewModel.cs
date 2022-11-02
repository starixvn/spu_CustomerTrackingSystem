namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using System;
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
            this.CategoryIds = data.CategoryIDs;
            this.CustStatus = data.CustStatus;
            this.Evidences = data.Evidence;
            this.AssociatedTypes = GetAssociationType(data.AssociationType);
            this.CustomerCategory = data.CustomerCategory;
            this.CustomerClass = data.CustomerClass > 0 ? data.CustomerClass.ToString() : null;
            this.IsProblemAccount = data.ParentIDs != null && data.ParentIDs.Split(',').Any(y => y == "50");
            this.IsPAProbation = data.IsPAProbation;
            this.AgentCTSCustID = data.AgentCTSCustID;
            if (!data.IsLicensee && data.AgentUsername != null)
                this.AgentUsername = data.AgentUsername;
            else
                this.AgentUsername = "-";

            if (!data.IsLicensee && data.AgentEvidence != null)
                this.AgentEvidence = data.AgentEvidence;
            else
                this.AgentEvidence = "-";
        }

        public int RowNumber { get; set; }

        public int? GroupID { get; set; }

        public string UserName { get; set; }       

        public long CTSCustID { get; set; }        

        public int CustID { get; set; }        

        public string CustStatus { get; set; }

        public string Evidences { get; set; }       

        public string AssociatedTypes { get; set; }

        public string CategoryIds { get; set; }

        public string CustomerCategory { get; set; }

        public string CustomerClass { get; set; }

        public bool IsProblemAccount { get; set; }

        public bool IsPAProbation { get; set; }

        public long AgentCTSCustID { get; set; }

        public string AgentUsername { get; set; }

        public string AgentEvidence { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }

        private string GetAssociationType(string assType)
        {
            var newAssType = "<div class='association-type-outer'>";
            newAssType += $"<div class='association-type-info'>{assType}</div>";

            if (assType != null && assType.Contains("Match"))
            {
                newAssType = assType.Replace("Match", $"<span class='association-detect-match' data-groupid={this.GroupID} data-custid={this.CustID} data-ctscustid={this.CTSCustID}>Match</span>");
            }
           
            newAssType += $"<div class='association-type-detail sub-icon - view-detail' id='sub-icon ' title='View Detail' data-groupid={this.GroupID} data-ctscustid={this.CTSCustID} data-username={this.UserName}><i class='icon-Detail'></i ></div>";
            newAssType += "</div>";
            return newAssType;
        }
    }
}
