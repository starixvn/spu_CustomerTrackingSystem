namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class CurrencyReasonViewModel
    {
        public CurrencyReasonViewModel()
        {
            this.ReasonItems = new List<CurrencyReasonItemViewModel>();
        }

        public int CurrencyId { get; set; }

        public IEnumerable<string> ScoreList { get; set; }

        public IEnumerable<CurrencyReasonItemViewModel> ReasonItems { get; set; }
    }
}
