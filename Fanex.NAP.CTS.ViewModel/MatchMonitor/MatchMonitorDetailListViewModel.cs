namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MatchMonitorDetailListViewModel : MMDetailFilterViewModel
    {
        public MatchMonitorDetailListViewModel() : base()
        {
            this.CTSCustIDList = string.Empty;
            this.HomeMatchItemList = Enumerable.Empty<MatchMonitorDetailItemViewModel>();
            this.AwayMatchItemList = Enumerable.Empty<MatchMonitorDetailItemViewModel>();
        }

        public MatchMonitorDetailListViewModel(MatchMonitorDetailListViewModel tickets, MMDetailFilterViewModel filter)
        {
            filter.CurrencyList = JsonConvert.DeserializeObject<IEnumerable<string>>(filter.CurrencyListString);
            filter.HDPList = JsonConvert.DeserializeObject<IEnumerable<decimal>>(filter.HDPListString);
            filter.ScoreList = JsonConvert.DeserializeObject<IEnumerable<string>>(filter.ScoreListString);
            filter.ReasonList = JsonConvert.DeserializeObject<IEnumerable<string>>(filter.ReasonListString);
            var reasonGroupIDDic = JsonConvert.DeserializeObject<IEnumerable<string>>(filter.ReasonGroupIDDicString);

            HomeMatchItemList = tickets.HomeMatchItemList.Where(x =>
                filter.CurrencyList.Contains(x.Currency ?? "-") &&
                filter.HDPList.Contains(x.HDP) &&
                filter.ScoreList.Contains(x.Score ?? "-") &&
                filter.ReasonList.Any(y => (x.ReasonList ?? "-").Contains(y)) &&
                reasonGroupIDDic.Contains(x.ADetectGroup ?? "-1")
                );
            AwayMatchItemList = tickets.AwayMatchItemList.Where(x =>
                filter.CurrencyList.Contains(x.Currency ?? "-") &&
                filter.HDPList.Contains(x.HDP) &&
                filter.ScoreList.Contains(x.Score ?? "-") &&
                filter.ReasonList.Any(y => (x.ReasonList ?? "-").Contains(y)) &&
                reasonGroupIDDic.Contains(x.ADetectGroup ?? "-1")
                );

            isFilter = filter.isFilter;
        }

        public long? LastSequenceID { get; set; }

        public string CTSCustIDList { get; set; }

        public IEnumerable<MatchMonitorDetailItemViewModel> HomeMatchItemList { get; set; }

        public IEnumerable<MatchMonitorDetailItemViewModel> AwayMatchItemList { get; set; }

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