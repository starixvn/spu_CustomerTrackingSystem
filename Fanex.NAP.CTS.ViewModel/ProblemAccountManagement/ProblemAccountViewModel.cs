namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor;    
    using System.Collections.Generic;

    public class ProblemAccountViewModel
    {
        public ProblemAccountViewModel()
        {
            SiteGroupList = new List<SubscriberGroupViewModel>();
            CategoryList = new List<CategoryGroupViewModel>();
        }

        public List<SubscriberGroupViewModel> SiteGroupList { get; set; }

        public List<CategoryGroupViewModel> CategoryList { get; set; }
    }
}
