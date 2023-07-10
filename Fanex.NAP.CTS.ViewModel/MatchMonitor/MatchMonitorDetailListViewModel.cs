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

        public MatchMonitorDetailListViewModel(MatchMonitorDetailListViewModel matchMonitorDetailList)
        {
            LastSequenceID = matchMonitorDetailList.LastSequenceID;
            CTSCustIDList = matchMonitorDetailList.CTSCustIDList;
            HomeMatchItemList = matchMonitorDetailList.HomeMatchItemList.ToList();
            AwayMatchItemList = matchMonitorDetailList.AwayMatchItemList.ToList();
            IsFilter = matchMonitorDetailList.IsFilter;
            MaxSequenceId = matchMonitorDetailList.MaxSequenceId;
            RecordsTotal = matchMonitorDetailList.RecordsTotal;
            CurrencyList = matchMonitorDetailList.CurrencyList;
            CurrencyIdList = matchMonitorDetailList.CurrencyIdList;
            HDPList = matchMonitorDetailList.HDPList;
            ScoreList = matchMonitorDetailList.ScoreList;
            ReasonList = matchMonitorDetailList.ReasonList;
            AmountList = matchMonitorDetailList.AmountList;
            GroupList = matchMonitorDetailList.GroupList;
            ReasonInfos = matchMonitorDetailList.ReasonInfos;
            ReasonGroupIDDic = matchMonitorDetailList.ReasonGroupIDDic;
            AGroupList = matchMonitorDetailList.AGroupList;
        }

        public void FilterItemMMDetail(MMDetailFilterViewModel filter, string site)
        {
            var reasonGroupIDDic = filter.AGroupList?.Select(x => x.Contains("-") ? "0" : x);

            HomeMatchItemList = HomeMatchItemList.Where(x => x.FilterItem(filter, reasonGroupIDDic, site)).ToList();
            AwayMatchItemList = AwayMatchItemList.Where(x => x.FilterItem(filter, reasonGroupIDDic, site)).ToList();
        }

        public void FilterItemMMDetailByReasonAGroup(MMDetailFilterViewModel filter)
        {
            var reasonGroupIDDic = filter.AGroupList?.Select(x => x.Contains("-") ? "0" : x);

            HomeMatchItemList = HomeMatchItemList.Where(x => x.FilterByReasonAGroup(filter.ReasonList, reasonGroupIDDic)).ToList();
            AwayMatchItemList = AwayMatchItemList.Where(x => x.FilterByReasonAGroup(filter.ReasonList, reasonGroupIDDic)).ToList();
        }

        public void FilterItemMMDetailBySite(string site)
        {
            HomeMatchItemList = HomeMatchItemList.Where(x => x.FilterBySite(site)).ToList();
            AwayMatchItemList = AwayMatchItemList.Where(x => x.FilterBySite(site)).ToList();

            var mdItemList = new List<MatchMonitorDetailItemViewModel>();
            mdItemList.AddRange(HomeMatchItemList);
            mdItemList.AddRange(AwayMatchItemList);

            ScoreList = mdItemList.Select(x => x.Score).Distinct().OrderBy(x => x).ToList();
            CurrencyList = mdItemList.Select(x =>
            {
                var isNotValid = string.IsNullOrEmpty(x.CurrencyName) || x.CurrencyId == null;
                return new { CurrencyId = isNotValid ? -1 : x.CurrencyId, CurrencyName = isNotValid ? "-" : x.CurrencyName };
            }).Distinct().OrderBy(x => x.CurrencyName).Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName)).ToList();
            HDPList = mdItemList.Select(x => x.BallHeadHDP()).Distinct().OrderBy(x => x).ToList();

            RecordsTotal = (HomeMatchItemList?.Count() ?? 0) + (AwayMatchItemList?.Count() ?? 0);

            BindReasons(mdItemList.Select(x => x.ReasonList).Distinct());

            var agroup = ReasonGroupIDDic.Where(x => ReasonList.Contains(x.Key)).SelectMany(x => x.Value).Distinct();
            GroupList = GroupList?.Where(x => x.GroupID != null ? agroup.Contains(x.GroupID ?? 0) : false).ToList();
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