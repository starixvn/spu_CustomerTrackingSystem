namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{  
    public class UpdateAssociatedGroupMonitorModel
    {
        public int GroupID { get; set; }

        public string GroupName { get; set; }

        public int ABI { get; set; }

        public int Ori { get; set; }

        public int AllCredit { get; set; }

        public string PACreditSites { get; set; }

        public int AllLicensee { get; set; }

        public string PALicenseeSites { get; set; }

        public string Sites { get; set; }

        public string AssociationTypes { get; set; }

        public string Remark { get; set; }     

        public int ModifiedBy { get; set; }
    }
}
