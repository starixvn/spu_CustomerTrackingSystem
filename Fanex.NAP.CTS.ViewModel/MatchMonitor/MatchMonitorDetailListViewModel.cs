namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System.Collections.Generic;
    using System.Linq;

    public class MatchMonitorDetailListViewModel
    {
        public MatchMonitorDetailListViewModel()
        {
            this.CTSCustIDList = string.Empty;
            this.HomeMatchItemList = Enumerable.Empty<MatchMonitorDetailItemViewModel>();
            this.AwayMatchItemList = Enumerable.Empty<MatchMonitorDetailItemViewModel>();
            this.ScoreList = Enumerable.Empty<string>();
            this.ReasonList = Enumerable.Empty<string>();
        }

        public string CTSCustIDList { get; set; }

        public IEnumerable<MatchMonitorDetailItemViewModel> HomeMatchItemList { get; set; }

        public IEnumerable<MatchMonitorDetailItemViewModel> AwayMatchItemList { get; set; }

        public IEnumerable<string> ScoreList { get; set; }

        public IEnumerable<string> ReasonList { get; set; }
        
    }
}