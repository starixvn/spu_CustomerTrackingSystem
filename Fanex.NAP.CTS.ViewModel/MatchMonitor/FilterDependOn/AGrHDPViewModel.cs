namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class AGrHDPViewModel
    {
        public AGrHDPViewModel()
        {
            this.HDPItems = new List<AGrHDPItemViewModel>();
        }

        public int AGr { get; set; }

        public IEnumerable<AGrHDPItemViewModel> HDPItems { get; set; }
    }
}
