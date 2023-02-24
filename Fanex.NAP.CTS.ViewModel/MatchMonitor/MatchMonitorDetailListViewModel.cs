namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;
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
            this.GroupList = Enumerable.Empty<GroupInfoViewModel>();
            this.ReasonInfos = string.Empty;            
        }        

        public string CTSCustIDList { get; set; }
    
        public IEnumerable<MatchMonitorDetailItemViewModel> HomeMatchItemList { get; set; }

        public IEnumerable<MatchMonitorDetailItemViewModel> AwayMatchItemList { get; set; }

        public IEnumerable<string> ScoreList { get; set; }

        public IEnumerable<string> ReasonList { get; set; }

        public IEnumerable<GroupInfoViewModel> GroupList { get; set; }

        public string ReasonInfos { get; set; }

        public void BindReasons(IEnumerable<string> rawReasons)
        {
            var reasons = new List<string>();
            
            foreach (var item in rawReasons)
            {
                if (!string.IsNullOrEmpty(item))
                    reasons.AddRange(item.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            this.ReasonList = reasons.Distinct();
            this.ReasonInfos = string.Join(", ", ReasonList);
        }

    }
}