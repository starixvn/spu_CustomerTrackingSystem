namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class CurrencyDangerViewModel
    {
        public CurrencyDangerViewModel()
        {
            this.DangerItems = new List<CurrencyDangerItemViewModel>();
        }

        public int CurrencyId { get; set; }

        public IEnumerable<CurrencyDangerItemViewModel> DangerItems { get; set; }
    }
}
