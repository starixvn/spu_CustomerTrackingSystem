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
            this.ReasonGroupIDDic = new Dictionary<string, List<int>>();
        }        

        public string CTSCustIDList { get; set; }
    
        public IEnumerable<MatchMonitorDetailItemViewModel> HomeMatchItemList { get; set; }

        public IEnumerable<MatchMonitorDetailItemViewModel> AwayMatchItemList { get; set; }

        public IEnumerable<string> ScoreList { get; set; }

        public IEnumerable<string> ReasonList { get; set; }

        public IEnumerable<GroupInfoViewModel> GroupList { get; set; }

        public string ReasonInfos { get; set; }

        public Dictionary<string, List<int>> ReasonGroupIDDic { get; set; }

        public void BindReasons(IEnumerable<string> rawReasons)
        {
            var reasons = new List<string>();
            
            foreach (var reason in rawReasons)
            {
                if (!string.IsNullOrEmpty(reason))
                    reasons.AddRange(reason.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            this.ReasonList = reasons.Distinct();
            this.ReasonInfos = string.Join(", ", ReasonList);

            var reasonGroupIDList = new List<int>();

            foreach (var reason in this.ReasonList)
            {
                var reasonGroupIDHs = this.HomeMatchItemList.Where(x => x.ReasonList.Contains(reason)).Select(x => x.ADetectGroupID);
                var reasonGroupIDAs = this.AwayMatchItemList.Where(x => x.ReasonList.Contains(reason)).Select(x => x.ADetectGroupID);

                reasonGroupIDList.Clear();
                reasonGroupIDList.AddRange(reasonGroupIDHs);
                reasonGroupIDList.AddRange(reasonGroupIDAs);

                this.ReasonGroupIDDic.Add(reason, reasonGroupIDList.Distinct().ToList());
            }

        }

    }
}