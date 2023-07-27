namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class ReasonAGroupViewModel
    {
        public ReasonAGroupViewModel()
        {
            this.AGroupItems = new List<ReasonAGroupItemViewModel>();
        }

        public string Reason { get; set; }

        public IEnumerable<ReasonAGroupItemViewModel> AGroupItems { get; set; }
    }
}
