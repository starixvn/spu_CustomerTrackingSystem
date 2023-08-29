namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class SubscriberTypeCurrencyViewModel
    {
        public SubscriberTypeCurrencyViewModel()
        {
            this.CurrencyItems = new List<SubscriberTypeCurrencyItemViewModel>();
        }

        public string SubscriberType { get; set; }

        public IEnumerable<SubscriberTypeCurrencyItemViewModel> CurrencyItems { get; set; }
    }
}
