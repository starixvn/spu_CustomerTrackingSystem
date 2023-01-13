namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model.AssociationDetection;

    public class DetectGroupAssociationItemViewModel
    {
        public DetectGroupAssociationItemViewModel(DetectAssociationMergeGroupModel data)
        {
            this.CTSCustID = data.CTSCustID;
            this.CustID = data.CustID;
            if (data.GroupID.HasValue)
            {
                this.GroupID = data.GroupID.Value;
                this.GroupName = data.GroupID.Value.ToString();
            }
            else
            {
                this.GroupID = 0;
                this.GroupName = "None";
            }
        }

        public int? GroupID { get; set; }

        public string GroupName { get; set; }

        public long CTSCustID { get; set; }

        public int CustID { get; set; }
    }
}
