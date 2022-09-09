namespace Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor
{
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using System.Collections.Generic;

    public class AssociatedGroupMonitorViewModel
    {
        public AssociatedGroupMonitorViewModel(AssociatedGroupMonitorModel data)
        {
            this.GroupID = data.GroupID;
            this.GroupName = data.GroupName;
            this.Sites = data.Sites;           
            var siteGroups = new List<string>();

            if (data.AllCredit == 1 || data.AllCredit == 2)            
                siteGroups.Add("<span class='view-site view-site-credit'>Credit</span>");
            

            if (data.AllLicensee == 1 || data.AllLicensee == 2)            
                siteGroups.Add("<span class='view-site view-site-licensee'>Licensee</span>");            

            this.SiteGroups = string.Join(",", siteGroups);

            var associatedTypes = new List<string>();
            if (data.HasAI)
                associatedTypes.Add("Betting Pattern");
            if (data.HasDevice)
                associatedTypes.Add("Device");
            if (data.HasIP)
                associatedTypes.Add("IP");
            if (data.HasManual)
                associatedTypes.Add("Manual");
            this.AssociatedTypes = string.Join(",", associatedTypes);

            this.ABI = data.ABI;
            this.TotalAccount = data.TotalAccount;
            this.Remark = data.Remark;
            this.CreatedDate = data.Created.ToString(Consts.FormatDateToString_MMMddyyyy);
            this.CreatedBy = data.CreatedBy;
        }
        public long GroupID { get; set; }

        public string GroupName { get; set; }

        public string SiteGroups { get; set; }

        public string Sites { get; set; }

        public string AssociatedTypes { get; set; }

        public int ABI { get; set; }

        public int TotalAccount { get; set; }

        public string Remark { get; set; }

        public string CreatedDate { get; set; }

        public string CreatedBy { get; set; }

    }
}
