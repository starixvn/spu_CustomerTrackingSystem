namespace Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor
{
    public class EditAssociatedGroupViewModel
    {
        public long GroupID { get; set; }

        public string GroupName { get; set; }

        public int ABI { get; set; }

        public bool IsAllCredit { get; set; }

        public bool IsAllLicensee { get; set; }

        public string Sites { get; set; }

        public string AssociationTypes { get; set; }

        public string Remark { get; set; }

        public int CreatedBy { get; set; }
    }
}
