namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model.AssociationDetection;

    public class DetectGroupAssociationItemViewModel
    {
        public DetectGroupAssociationItemViewModel(DetectAssociationMergeGroupModel data)
        {
            this.CTSCustID = data.CTSCustID;
            this.CustID = data.CustID;
            this.GroupID = data.GroupID.HasValue ? data.GroupID.Value.ToString() : string.Empty;
        }

        public string GroupID { get; set; }

        public long CTSCustID { get; set; }

        public int CustID { get; set; }
    }
}
