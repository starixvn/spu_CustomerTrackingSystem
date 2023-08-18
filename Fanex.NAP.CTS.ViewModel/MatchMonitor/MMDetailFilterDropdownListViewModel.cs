namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn;
    using System.Collections.Generic;

    public class MMDetailFilterDropdownListViewModel
    {
        public MMDetailFilterDropdownListViewModel()
        {            
            this.SubscriberTypeCurrencyList = new List<SubscriberTypeCurrencyViewModel>();          
            this.Danger = new DangerViewModel();
            this.TicketStatusList = new List<TicketStatusViewModel>();
        }     

        public IEnumerable<SubscriberTypeCurrencyViewModel> SubscriberTypeCurrencyList { get; set; }      
      
        public DangerViewModel Danger { get; set; }

        public IEnumerable<TicketStatusViewModel> TicketStatusList { get; set; }

    }
}
