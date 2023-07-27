namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class DangerCurrencyViewModel
    {
        public DangerCurrencyViewModel()
        {
            this.CurrencyItems = new List<DangerCurrencyItemViewModel>();
        }

        public int DangerType { get; set; }

        public int DangerValue { get; set; }

        public IEnumerable<DangerCurrencyItemViewModel> CurrencyItems { get; set; }
        
    }
}
