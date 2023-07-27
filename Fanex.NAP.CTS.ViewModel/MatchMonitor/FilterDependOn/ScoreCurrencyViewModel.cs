namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class ScoreCurrencyViewModel
    {
        public ScoreCurrencyViewModel()
        {
            this.CurrencyItems = new List<ScoreCurrencyItemViewModel>();
        }

        public string Score { get; set; }

        public IEnumerable<ScoreCurrencyItemViewModel> CurrencyItems { get; set; }
    }
}
