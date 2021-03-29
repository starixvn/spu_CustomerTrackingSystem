namespace Fanex.NAP.CTS.ViewModel.Common
{
    using System.Collections.Generic;

    public class SubscriberGroupViewModel
    {
        public SubscriberGroupViewModel()
        {
            SubscriberList = new List<SubscriberViewModel>();
        }

        public int SubscriberGroupID { get; set; }

        public string SubscriberGroupName { get; set; }

        public IEnumerable<SubscriberViewModel> SubscriberList { get; set; }
    }
}
