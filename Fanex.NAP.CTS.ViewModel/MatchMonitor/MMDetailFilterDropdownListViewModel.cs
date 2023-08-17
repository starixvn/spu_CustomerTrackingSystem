namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn;
    using System.Collections.Generic;

    public class MMDetailFilterDropdownListViewModel
    {
        public MMDetailFilterDropdownListViewModel()
        {
            this.ScoreCurrencyList = new List<ScoreCurrencyViewModel>();
            this.SubscriberTypeCurrencyList = new List<SubscriberTypeCurrencyViewModel>();
            this.DangerCurrencyList = new List<DangerCurrencyViewModel>();
            this.CurrencyReasonList = new List<CurrencyReasonViewModel>();
            this.CurrencyAGroupList = new List<CurrencyAGroupViewModel>();
            this.ReasonAGroupList = new List<ReasonAGroupViewModel>();
            this.AGrMGrBlankList = new List<AGrMGrViewModel>();
            this.Danger = new DangerViewModel();
            this.TicketStatusList = new List<TicketStatusViewModel>();
        }

        public IEnumerable<ScoreCurrencyViewModel> ScoreCurrencyList { get; set; }

        public IEnumerable<SubscriberTypeCurrencyViewModel> SubscriberTypeCurrencyList { get; set; }

        public IEnumerable<DangerCurrencyViewModel> DangerCurrencyList { get; set; }

        public IEnumerable<CurrencyReasonViewModel> CurrencyReasonList { get; set; }

        public IEnumerable<CurrencyAGroupViewModel> CurrencyAGroupList { get; set; }

        public IEnumerable<ReasonAGroupViewModel> ReasonAGroupList { get; set; }
       
        public IEnumerable<AGrMGrViewModel> AGrMGrBlankList { get; set; }

        public DangerViewModel Danger { get; set; }

        public IEnumerable<TicketStatusViewModel> TicketStatusList { get; set; }

    }
}
