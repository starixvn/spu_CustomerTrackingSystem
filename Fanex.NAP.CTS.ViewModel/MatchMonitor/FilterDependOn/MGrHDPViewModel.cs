namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class MGrHDPViewModel
    {
        public MGrHDPViewModel()
        {
            this.HDPItems = new List<MGrHDPItemViewModel>();
        }

        public int MGr { get; set; }

        public IEnumerable<MGrHDPItemViewModel> HDPItems { get; set; }
    }
}
