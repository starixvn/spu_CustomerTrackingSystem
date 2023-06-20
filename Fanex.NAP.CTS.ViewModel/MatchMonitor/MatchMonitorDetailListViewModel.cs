namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

            var reasonGroupIDDic = filter.AGroupList?.Select(x => x.Contains("-") ? "0" : x);

            HomeMatchItemList = tickets.HomeMatchItemList.ToList().Where(x => FilterItem(x, filter, reasonGroupIDDic, site)).ToList();
            AwayMatchItemList = tickets.AwayMatchItemList.ToList().Where(x => FilterItem(x, filter, reasonGroupIDDic, site)).ToList();

            ScoreList = tickets.ScoreList;
            CurrencyList = tickets.CurrencyList;
            HDPList = tickets.HDPList;
            CTSCustIDList = tickets.CTSCustIDList;
            RecordsTotal = tickets.RecordsTotal;
            GroupList = tickets.GroupList;
            ReasonList = tickets.ReasonList;
            ReasonInfos = tickets.ReasonInfos;
            LastSequenceID = tickets.LastSequenceID;
        }

        private bool FilterItem(MatchMonitorDetailItemViewModel x, MMDetailFilterViewModel filter, IEnumerable<string> reasonGroupIDDic, string site)
        {
            return
                (filter.CurrencyIdList?.Contains(x.CurrencyId) ?? true) &&
                (filter.HDPList?.Contains(x.BallHeadHDP()) ?? true) &&
                (filter.ScoreList?.Contains(x.Score ?? "-") ?? true) &&
                (filter.ReasonList?.Any(y => (x.ReasonList ?? "-").Contains(y)) ?? true) &&
                (reasonGroupIDDic?.Contains(x.ADetectGroupID.ToString()) ?? true) &&
                (filter.AmountList == null || filter.AmountList.Count() < 2 ? true : x.Amount >= filter.AmountList[0] && x.Amount <= filter.AmountList[1]) &&
                x.FilterBySite(site);
        }

        public MatchMonitorDetailListViewModel(MatchMonitorDetailListViewModel tickets, string site)
        {
            HomeMatchItemList = tickets.HomeMatchItemList.Where(x => x.FilterBySite(site)).ToList();
            AwayMatchItemList = tickets.AwayMatchItemList.Where(x => x.FilterBySite(site)).ToList();

            var mdItemList = new List<MatchMonitorDetailItemViewModel>();
            mdItemList.AddRange(HomeMatchItemList);
            mdItemList.AddRange(AwayMatchItemList);

            ScoreList = mdItemList.Select(x => x.Score).Distinct().OrderBy(x => x).ToList();
            CurrencyList = mdItemList.Select(x =>
            {
                var isNotValid = string.IsNullOrEmpty(x.CurrencyName) || x.CurrencyId == null;
                return new { CurrencyId = isNotValid ? -1 : x.CurrencyId, CurrencyName = isNotValid ? "-" : x.CurrencyName };
            }).Distinct().OrderBy(x => x.CurrencyName).ToDictionary(x => x.CurrencyName, y => y.CurrencyId);
            HDPList = mdItemList.Select(x => x.BallHeadHDP()).Distinct().OrderBy(x => x).ToList();

            LastSequenceID = tickets.LastSequenceID;
            CTSCustIDList = tickets.CTSCustIDList;
            IsFilter = tickets.IsFilter;
            RecordsTotal = (HomeMatchItemList?.Count() ?? 0) + (AwayMatchItemList?.Count() ?? 0);
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
            this.ReasonGroupIDDic = new Dictionary<string, List<int>>();

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