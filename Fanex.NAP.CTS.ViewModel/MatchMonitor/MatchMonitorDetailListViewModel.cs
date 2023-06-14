namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MatchMonitorDetailListViewModel : MMDetailFilterViewModel
    {
        public MatchMonitorDetailListViewModel() : base()
        {
            this.CTSCustIDList = string.Empty;
            this.HomeMatchItemList = new List<MatchMonitorDetailItemViewModel>();
            this.AwayMatchItemList = new List<MatchMonitorDetailItemViewModel>();
        }

        public MatchMonitorDetailListViewModel(MatchMonitorDetailListViewModel tickets, MMDetailFilterViewModel filter, string site)
        {
            if (tickets == null)
            {
                return;
            }

            var reasonGroupIDDic = filter.AGroupList.Select(x => x.Contains("-") ? "0" : x);

            HomeMatchItemList = tickets.HomeMatchItemList.ToList().Where(x =>
                filter.CurrencyList.Contains(x.Currency ?? "-") &&
                filter.HDPList.Contains(x.HDP) &&
                filter.ScoreList.Contains(x.Score ?? "-") &&
                filter.ReasonList.Any(y => (x.ReasonList ?? "-").Contains(y)) &&
                reasonGroupIDDic.Contains(x.ADetectGroupID.ToString()) &&
                x.FilterBySite(site)
                ).ToList();
            AwayMatchItemList = tickets.AwayMatchItemList.ToList().Where(x =>
                filter.CurrencyList.Contains(x.Currency ?? "-") &&
                filter.HDPList.Contains(x.HDP) &&
                filter.ScoreList.Contains(x.Score ?? "-") &&
                filter.ReasonList.Any(y => (x.ReasonList ?? "-").Contains(y)) &&
                reasonGroupIDDic.Contains(x.ADetectGroupID.ToString()) &&
                x.FilterBySite(site)
                ).ToList();

            IsFilter = true;
        }

        public MatchMonitorDetailListViewModel(MatchMonitorDetailListViewModel tickets, string site)
        {
            HomeMatchItemList = tickets.HomeMatchItemList.Where(x => x.FilterBySite(site)).ToList();
            AwayMatchItemList = tickets.AwayMatchItemList.Where(x => x.FilterBySite(site)).ToList();

            var mdItemList = new List<MatchMonitorDetailItemViewModel>();
            mdItemList.AddRange(HomeMatchItemList);
            mdItemList.AddRange(AwayMatchItemList);

            ScoreList = mdItemList.Select(x => x.Score).Distinct().OrderBy(x => x).ToList();
            CurrencyList = mdItemList.Select(x => string.IsNullOrEmpty(x.Currency) ? "-" : x.Currency).Distinct().OrderBy(x => x).ToList();
            HDPList = mdItemList.Select(x => x.HDP).Distinct().OrderBy(x => x).ToList();

            LastSequenceID = tickets.LastSequenceID;
            CTSCustIDList = tickets.CTSCustIDList;
            IsFilter = tickets.IsFilter;
            RecordsTotal = tickets.RecordsTotal;
            GroupList = tickets.GroupList;

            BindReasons(mdItemList.Select(x => x.ReasonList).Distinct());
        }

        public long? LastSequenceID { get; set; }

        public string CTSCustIDList { get; set; }

        public List<MatchMonitorDetailItemViewModel> HomeMatchItemList { get; set; }

        public List<MatchMonitorDetailItemViewModel> AwayMatchItemList { get; set; }

        public void BindReasons(IEnumerable<string> rawReasons)
        {
            var reasons = new List<string>();

            foreach (var reason in rawReasons)
            {
                if (!string.IsNullOrEmpty(reason))
                    reasons.AddRange(reason.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            this.ReasonList = reasons.Distinct().ToList();
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