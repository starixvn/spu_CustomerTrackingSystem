namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn;
    using System.Collections.Generic;

    public class MMDetailFilterDropdownListViewModel
    {
        public MMDetailFilterDropdownListViewModel()
        {            
            this.SubscriberTypeList = new List<string>();
            this.CCGroupList = new List<CCGroupViewModel>();
            this.DangerList = new DangerViewModel();
            this.TicketStatusList = new List<TicketStatusViewModel>();
        }

        public IEnumerable<string> SubscriberTypeList { get; set; }

        public IEnumerable<CCGroupViewModel> CCGroupList { get; set; }

        public DangerViewModel DangerList { get; set; }

        public IEnumerable<TicketStatusViewModel> TicketStatusList { get; set; }

    }
}
