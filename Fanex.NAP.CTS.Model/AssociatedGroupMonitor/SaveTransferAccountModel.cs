namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{
    public class SaveTransferAccountModel
    {
        public string UserNames { get; set; }

        public string CTSCustIDs { get; set; }

        public long FromGroupID { get; set; }

        public long ToGroupID { get; set; }

        public int ModifiedBy { get; set; }

        public int ABI { get; set; }

        public int Ori { get; set; }
    }
}
