namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class AGrMGrViewModel
    {
        public AGrMGrViewModel()
        {
            this.MGrItems = new List<AGrMGrItemViewModel>();
        }

        public int AGr { get; set; }

        public IEnumerable<AGrMGrItemViewModel> MGrItems { get; set; }
        
    }
}
