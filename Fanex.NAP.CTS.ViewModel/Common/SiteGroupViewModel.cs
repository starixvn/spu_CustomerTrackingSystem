namespace Fanex.NAP.CTS.ViewModel.Common
{
    using System.Collections.Generic;

    public class SiteGroupViewModel
    {
        public SiteGroupViewModel()
        {
            SiteList = new List<SiteViewModel>();
        }       

        public string SubscriberGroupName { get; set; }

        public IEnumerable<SiteViewModel> SiteList { get; set; }
    }
}
