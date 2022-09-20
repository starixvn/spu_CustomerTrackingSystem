namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{    
    public class AddAssociatedAccountModel
    {
        public long GroupID { get; set; }

        public string CustIDs { get; set; }

        public string CTSCustIDs { get; set; }

        public bool IsAuto { get; set; }

        public string UserNames { get; set; }

        public string Comment { get; set; }

        public int CreatedBy { get; set; }

        public int ABI { get; set; }
    }
}
