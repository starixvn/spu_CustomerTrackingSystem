namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{
    public class SetProblemCategoryByGroupIDModel
    {    
        public long GroupID { get; set; }
    
        public int PACategoryID { get; set; }

        public string PACreditSites { get; set; }

        public string PALicenseeSites { get; set; }
      
        public int ModifiedBy { get; set; }

    }
}