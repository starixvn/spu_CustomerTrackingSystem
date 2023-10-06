namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn;
    using System.Collections.Generic;

    public class MMDetailFilterDropdownListViewModel
    {
        public MMDetailFilterDropdownListViewModel()
        {            
            this.SubscriberTypeList = new List<string>();
            this.CCList = new List<int>();
            this.DangerList = new DangerViewModel();
            this.TicketStatusList = new List<TicketStatusViewModel>();
        }

        public IEnumerable<string> SubscriberTypeList { get; set; }

        public IEnumerable<int> CCList { get; set; }

        public DangerViewModel DangerList { get; set; }

        public IEnumerable<TicketStatusViewModel> TicketStatusList { get; set; }

    }
}
