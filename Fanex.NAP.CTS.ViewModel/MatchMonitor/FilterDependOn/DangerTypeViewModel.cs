namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class DangerTypeViewModel
    {
        public DangerTypeViewModel()
        {
            this.DangerItems = new List<DangerItemViewModel>();
        }

        public string DangerTypeName { get; set; }

        public IEnumerable<DangerItemViewModel> DangerItems { get; set; }
    }
}
