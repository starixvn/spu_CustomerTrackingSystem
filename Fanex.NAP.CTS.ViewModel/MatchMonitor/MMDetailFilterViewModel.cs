namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MMDetailFilterViewModel
    {
        public MMDetailFilterViewModel()
        {
            this.ReasonInfos = string.Empty;
        }

        public MMDetailFilterViewModel(MMTicketDetailInfoModel mMTicket, MMDetailFilterViewModel fraudFilter, string site)
        {
            switch (site)
            {
                case MMSites.All:
                    {
                        RecordsTotal = mMTicket.TotalTicketAll;
                        break;
                    }
                case MMSites.Credit:
                    {
                        RecordsTotal = mMTicket.TotalTicketCredit;
                        break;
                    }
                case MMSites.Licensee:
                    {
                        RecordsTotal = mMTicket.TotalTicketLicensee;
                        break;
                    }
            }

            MaxSequenceId = mMTicket.MaxSequenceId;
            CurrencyList = mMTicket.FilterDetail.CurrencyList?.Where(x => FilterBySite(x, site)).Select(x =>
            {
                var isNotValid = string.IsNullOrEmpty(x.CurrencyName) || x.CurrencyId == null;
                return new { CurrencyId = isNotValid ? -1 : x.CurrencyId, CurrencyName = isNotValid ? "-" : x.CurrencyName };
            }).Distinct().OrderBy(x => x.CurrencyName).Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName)).ToList() ?? new List<KeyValuePair<int, string>>() { new KeyValuePair<int, string>(-1, "-") };
            HDPList = mMTicket.FilterDetail.HDPList?.Where(x => FilterBySite(x, site)).Select(x => x.Hdp).OrderBy(x => x).ToList() ?? new List<decimal>() { };
            ScoreList = mMTicket.FilterDetail.ScoreList?.Any() ?? false ? mMTicket.FilterDetail.ScoreList.Where(x => FilterBySite(x, site)).Select(x => string.Format($"{x.LiveHomeScore} - {x.LiveAwayScore}")).OrderBy(x => x).ToList() : new List<string>() { "-" };

            var reasonList = fraudFilter.ReasonList?.ToList() ?? new List<string>();
            var groupList = fraudFilter.GroupList?.ToList() ?? new List<GroupInfoViewModel>();
            var reasonGroupIDDic = fraudFilter?.ReasonGroupIDDic ?? new Dictionary<string, List<int>>();

            if (fraudFilter.RecordsTotal < RecordsTotal)
            {
                if (!reasonList.Any(x => x == "-")) reasonList.Insert(0, "-");
                if (!reasonGroupIDDic.Any(x => x.Key == "-")) reasonGroupIDDic.Add("-", new List<int>() { 0 });
                if (!groupList.Any(x => x.GroupName == "-")) groupList.Insert(0, new GroupInfoViewModel
                {
                    GroupID = 0,
                    GroupName = "-"
                });
            }

            ReasonList = reasonList;
            ReasonInfos = string.Join(", ", ReasonList.Where(x => !x.Contains("-")));
            ReasonGroupIDDic = reasonGroupIDDic.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            var agroup = ReasonGroupIDDic.Where(x => ReasonList.Contains(x.Key)).SelectMany(x => x.Value).Distinct();
            GroupList = groupList?.Where(x => x.GroupID != null ? agroup.Contains(x.GroupID ?? 0) : false).ToList();
        }

        protected MMDetailFilterViewModel(MMDetailFilterViewModel filter)
        {
            IsFilter = filter.IsFilter;
            MaxSequenceId = filter.MaxSequenceId;
            RecordsTotal = filter.RecordsTotal;
            CurrencyList = filter.CurrencyList;
            CurrencyIdList = filter.CurrencyIdList;
            HDPList = filter.HDPList;
            ScoreList = filter.ScoreList;
            ReasonList = filter.ReasonList;
            AmountList = filter.AmountList;
            GroupList = filter.GroupList;
            ReasonInfos = filter.ReasonInfos;
            ReasonGroupIDDic = filter.ReasonGroupIDDic;
            AGroupList = filter.AGroupList;
            SubscriberTypeList = filter.SubscriberTypeList;
        }

        public bool IsFilter { get; set; }

        public long? MaxSequenceId { get; set; }

        public long RecordsTotal { get; set; }

        public List<string> SubscriberTypeList { get; set; }

        public List<KeyValuePair<int, string>> CurrencyList { get; set; }

        public List<int> CurrencyIdList { get; set; }

        public List<decimal> HDPList { get; set; }

        public List<string> ScoreList { get; set; }

        public List<int> Danger1List { get; set; }

        public List<int> Danger2List { get; set; }

        public List<int> Danger3List { get; set; }

        public List<int> Danger4List { get; set; }

        public List<int> Danger5List { get; set; }

        public List<int> StatusList { get; set; }

        public List<string> ReasonList { get; set; }

        private List<decimal> amountList { get; set; }

        public List<decimal> AmountList
        {
            get { return amountList; }
            set
            {
                if (value?.Count() > 0)
                {
                    amountList = value;
                    amountList[0] = amountList[0] < 0 ? 0 : amountList[0];
                    amountList[0] = amountList[0] > 922337203685477 ? 922337203685477 : amountList[0];
                    amountList[1] = amountList[1] < 0 ? 0 : amountList[1];
                    amountList[1] = amountList[1] > 922337203685477 ? 922337203685477 : amountList[1];
                }
            }
        }

        public List<GroupInfoViewModel> GroupList { get; set; }

        public string ReasonInfos { get; set; }

        public Dictionary<string, List<int>> ReasonGroupIDDic { get; set; }

        public List<string> AGroupList { get; set; }

        public List<string> MGroupList { get; set; }

        public List<int> TicketStatusList { get; set; }

        public MMDetailFilterViewModel Clone() => new MMDetailFilterViewModel(this);

        public bool IsValidFilterTicketLatest(MMDetailFilterViewModel filterLastest)
        {
            if (IsFilter)
            {
                var count = 0;

                if (filterLastest.HDPList != null && !filterLastest.HDPList.Except(HDPList).Any())
                {
                    HDPList = null;
                    count++;
                }
               
                if (filterLastest.ScoreList != null && !filterLastest.ScoreList.Except(ScoreList).Any())
                {
                    ScoreList = null;
                    count++;
                }

                if (filterLastest.CurrencyList != null && !filterLastest.CurrencyList.Select(x => x.Key).Except(CurrencyIdList).Any())
                {
                    CurrencyIdList = null;
                    count++;
                }

                if (filterLastest.ReasonGroupIDDic != null && !filterLastest.ReasonGroupIDDic.Where(x => ReasonList.Contains(x.Key)).SelectMany(x => x.Value).Distinct()
                    .ToList().ConvertAll(x => x.ToString()).Except(AGroupList).Any())
                {
                    AGroupList = null;
                    count++;
                }

                if (filterLastest.ReasonList != null && !filterLastest.ReasonList.Except(ReasonList).Any())
                {
                    ReasonList = null;
                    count++;
                }

                if (!AmountList?.Any() ?? true)
                {
                    count++;
                }

                if (filterLastest.TicketStatusList != null && !filterLastest.TicketStatusList.Except(TicketStatusList).Any())
                {
                    TicketStatusList = null;
                    count++;
                }

                IsFilter = count < 7;
            }

            return IsFilter;
        }

        public bool IsOnlyFraudTicket()
        {
            return (!ReasonList?.Contains("-") ?? false) || !((AGroupList?.Contains("-") ?? true) || (AGroupList?.Contains("0") ?? true));
        }

        private bool FilterBySite(MMdetailFilterInforTypeBaseModel x, string site)
        {
            if (site == MMSites.All) return true;

            if (site == MMSites.Credit) return x.IsCredit == 1;

            if (site == MMSites.Licensee) return x.IsLicensee == 1;

            return false;
        }

    }
}
