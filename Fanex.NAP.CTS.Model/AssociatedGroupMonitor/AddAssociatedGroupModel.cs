namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{
    public class AddAssociatedGroupModel
    {
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
