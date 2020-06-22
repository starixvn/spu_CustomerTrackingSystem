namespace Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor
{
    using System.Collections.Generic;

    public class SubscriberGroupViewModel
    {
        public SubscriberGroupViewModel()
        {
            SiteList = new List<SiteViewModel>();
        }

        public int SubscriberGroupID { get; set; }

        public string SubscriberGroupName { get; set; }

        public IEnumerable<SiteViewModel> SiteList { get; set; }

    }
}