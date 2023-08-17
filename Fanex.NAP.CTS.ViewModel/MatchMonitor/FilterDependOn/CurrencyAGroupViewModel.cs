namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class CurrencyAGroupViewModel
    {
        public CurrencyAGroupViewModel()
        {
            this.AGroupItems = new List<CurrencyAGroupItemViewModel>();
        }

        public int CurrencyId { get; set; }

        public IEnumerable<CurrencyAGroupItemViewModel> AGroupItems { get; set; }

    }
}
