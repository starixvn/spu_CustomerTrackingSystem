namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{
    public class SaveTransferAccountModel
    {
        public string CTSCustIDs { get; set; }

        public long FromGroupID { get; set; }

        public long ToGroupID { get; set; }

        public int ModifiedBy { get; set; }
    }
}
