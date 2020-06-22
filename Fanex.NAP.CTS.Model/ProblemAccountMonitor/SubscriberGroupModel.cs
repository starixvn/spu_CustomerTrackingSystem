namespace Fanex.NAP.CTS.Model.ProblemAccountMonitor
{
    public class SubscriberGroupModel
    {
        public int SiteID { get; set; }

        public string SiteName { get; set; }

        public int SubscriberGroupID { get; set; }

        public string SubscriberGroupName { get; set; }

        public int ParentID { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
