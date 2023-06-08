namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Core;
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

        public MatchMonitorDetailListViewModel(MatchMonitorDetailListViewModel tickets, MMDetailFilterViewModel filter, string site)
        {
            if (tickets == null)
            {
                return;
            }

            var reasonGroupIDDic = filter.AGroupList.Select(x => x.Contains("-") ? "0" : x);

            HomeMatchItemList = tickets.HomeMatchItemList.Where(x =>
                filter.CurrencyList.Contains(x.Currency ?? "-") &&
                filter.HDPList.Contains(x.HDP) &&
                filter.ScoreList.Contains(x.Score ?? "-") &&
                filter.ReasonList.Any(y => (x.ReasonList ?? "-").Contains(y)) &&
                reasonGroupIDDic.Contains(x.ADetectGroupID.ToString()) &&
                x.FilterBySite(site)
                );
            AwayMatchItemList = tickets.AwayMatchItemList.Where(x =>
                filter.CurrencyList.Contains(x.Currency ?? "-") &&
                filter.HDPList.Contains(x.HDP) &&
                filter.ScoreList.Contains(x.Score ?? "-") &&
                filter.ReasonList.Any(y => (x.ReasonList ?? "-").Contains(y)) &&
                reasonGroupIDDic.Contains(x.ADetectGroupID.ToString()) &&
                x.FilterBySite(site)
                );

            IsFilter = true;
        }

        public MatchMonitorDetailListViewModel(MatchMonitorDetailListViewModel tickets, string site)
        {
            LastSequenceID = tickets.LastSequenceID;
            CTSCustIDList = tickets.CTSCustIDList;
            IsFilter = tickets.IsFilter;
            RecordsTotal = tickets.RecordsTotal;
            CurrencyList = tickets.CurrencyList;
            HDPList = tickets.HDPList;
            ScoreList = tickets.ScoreList;
            ReasonList = tickets.ReasonList;
            GroupList = tickets.GroupList;
            ReasonInfos = tickets.ReasonInfos;
            ReasonGroupIDDic = tickets.ReasonGroupIDDic;
            HomeMatchItemList = tickets.HomeMatchItemList.Where(x => x.FilterBySite(site));
            AwayMatchItemList = tickets.AwayMatchItemList.Where(x => x.FilterBySite(site));
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