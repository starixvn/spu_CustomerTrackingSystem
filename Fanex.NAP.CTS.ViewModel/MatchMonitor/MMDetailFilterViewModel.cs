namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;   
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MMDetailFilterViewModel
    {
        public MMDetailFilterViewModel()
        {
            this.ReasonInfos = string.Empty;
        }

        public MMDetailFilterViewModel(MMTicketDetailInfoModel mMTicket, string site)
        {           
            switch (site)
            {
                case MMSites.All:
                    {
                        this.RecordsTotal = mMTicket.TotalTicketAll; break;
                    }
                case MMSites.Credit:
                    {
                        this.RecordsTotal = mMTicket.TotalTicketCredit; break;
                    }
                case MMSites.Licensee:
                    {
                        this.RecordsTotal = mMTicket.TotalTicketLicensee; break;
                    }
            }

            this.MaxSequenceId = mMTicket.MaxSequenceId;
           
            this.SubscriberTypeList = new List<string>();
            this.CurrencyList = new List<KeyValuePair<int, string>>();
            this.HDPList = new List<decimal>();
            this.BetTeamList = new List<string>();
            this.ScoreList = new List<string>();
            this.ReasonList = new List<string>();
            this.GroupList = new List<GroupInfoViewModel>();
            this.MGroupList = new List<string>();
        }

        protected MMDetailFilterViewModel(MMDetailFilterViewModel filter)
        {
            this.IsFilter = filter.IsFilter;
            this.MaxSequenceId = filter.MaxSequenceId;
            this.RecordsTotal = filter.RecordsTotal;
            this.ScoreList = filter.ScoreList;
            this.Danger1List = filter.Danger1List;
            this.Danger2List = filter.Danger2List;
            this.Danger3List = filter.Danger3List;
            this.Danger4List = filter.Danger4List;
            this.Danger5List = filter.Danger5List;
            this.CurrencyList = filter.CurrencyList;
            this.CurrencyIdList = filter.CurrencyIdList;
            this.ReasonInfos = filter.ReasonInfos;
            this.ReasonGroupIDDic = filter.ReasonGroupIDDic;
            this.AGroupList = filter.AGroupList;
            this.MGroupList = filter.MGroupList;
            this.ReasonList = filter.ReasonList;
            this.GroupList = filter.GroupList;
            this.HDPList = filter.HDPList;
            this.BetTeamList = filter.BetTeamList;
            this.AmountList = filter.AmountList;
            this.TicketStatusList = filter.TicketStatusList;
            this.SubscriberTypeList = filter.SubscriberTypeList;
            this.IsDangerNone = filter.IsDangerNone;
            this.DropdownFilterList = filter.DropdownFilterList;
        }

        public bool IsFilter { get; set; }

        public long? MaxSequenceId { get; set; }

        public long RecordsTotal { get; set; }

        public List<string> SubscriberTypeList { get; set; }

        public List<KeyValuePair<int, string>> CurrencyList { get; set; }

        public List<int> CurrencyIdList { get; set; }

        public List<decimal> HDPList { get; set; }

        public List<string> BetTeamList { get; set; }

        public List<string> ScoreList { get; set; }

        public bool IsDangerNone { get; set; }

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

        public MMDetailFilterDropdownListViewModel DropdownList { get; set; }

        public IEnumerable<MMFilterItemViewModel> DropdownFilterList { get; set; }

        public MMDetailFilterViewModel Clone() => new MMDetailFilterViewModel(this);
      
        public void BindDropdownFilterListByMainTicketInfo(IEnumerable<MMDetailFilterTicketInfoModel> tiItemList, bool liveIndicator)
        {
            var tiItemViewList = tiItemList.Select(x => new MMDetailFilterTicketInfoViewModel(x, liveIndicator));

            var hashSet = new HashSet<string>();

            foreach (var item in tiItemList)
            {
                hashSet.Add(JsonConvert.SerializeObject(new MMFilterItemViewModel
                {
                    CurrencyId = item.CurrencyId,
                    CurrencyName = item.CurrencyName,
                    Danger1 = item.Danger1,
                    Danger2 = item.Danger2,
                    Danger3 = item.Danger3,
                    Danger4 = item.Danger4,
                    Danger5 = item.Danger5,
                    Score = liveIndicator ? $"{item.LiveHomeScore} - {item.LiveAwayScore}" : "-",
                    SubscriberType = item.IsLicensee ? "L" : "C",
                    BallHeadHDP = item.Hdp,
                    BallHeadBetTeam = item.BetTeam,
                    TicketStatus = item.TicketStatus
                }));
            }

            DropdownFilterList = hashSet.Select(JsonConvert.DeserializeObject<MMFilterItemViewModel>);           
        }        

        public void BindDropdownFilterListByFraudTicket(MMDetailFilterViewModel fraudTicketModel)
        {
            if (this.DropdownList == null)
                this.DropdownList = new MMDetailFilterDropdownListViewModel();

            DropdownFilterList = DropdownFilterList.Union(fraudTicketModel.DropdownFilterList);            

            this.CurrencyList = DropdownFilterList.Select(x =>
            {
                return new { CurrencyId = string.IsNullOrEmpty(x.CurrencyName) ? -1 : x.CurrencyId, CurrencyName = string.IsNullOrEmpty(x.CurrencyName) ? "-" : x.CurrencyName };
            }).Distinct(x => x.CurrencyId)
                  .OrderBy(x => x.CurrencyName)
                  .Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName))
                  .ToList();

            this.HDPList = DropdownFilterList.Select(x => x.BallHeadHDP).Distinct().OrderBy(x => x).ToList();

            this.BetTeamList = DropdownFilterList.Select(x => x.BallHeadBetTeam).Distinct().OrderBy(x => x).ToList();

            this.ScoreList = DropdownFilterList.Select(x => x.Score).Distinct(x => x).OrderBy(x => x).ToList();

            this.GroupList = DropdownFilterList.Select(x => new GroupInfoViewModel
            {
                GroupID = x.ADetectGroupID,
                GroupName = x.ADetectGroup
            }).Distinct(x=>x.GroupID).OrderBy(x => x.GroupID).ToList();

            this.ReasonList = DropdownFilterList.Select(x => x.ReasonItem).Distinct().OrderBy(x => x).ToList();

            if (DropdownFilterList.Any()) 
                this.MGroupList = new List<string>() { "-" };            

            var subscriberTypeList = DropdownFilterList.Select(x => x.SubscriberType).Distinct().ToList();

            if (subscriberTypeList.Count() == 2)
                subscriberTypeList.Insert(0, "A");

            DropdownList.SubscriberTypeList = subscriberTypeList;

            this.DropdownList.TicketStatusList = DropdownFilterList.Select(x => x.TicketStatus).Distinct()
                        .Select(x => new TicketStatusViewModel
                        {
                            TicketStatus = x,
                            TicketStatusName = Enum.GetName(typeof(StatusTicket), x)
                        }).OrderBy(x => x.StatusOrderIdx);

            #region Danger List                 
            var dangerList = DropdownFilterList.Where(x => x.DangerToString != "-");
            var currency1 = dangerList.Where(x => x.Danger1 > 0).Select(x => x.CurrencyName).Distinct();
            var currency2 = dangerList.Where(x => x.Danger2 > 0).Select(x => x.CurrencyName).Distinct();
            var currency3 = dangerList.Where(x => x.Danger3 > 0).Select(x => x.CurrencyName).Distinct();
            var currency4 = dangerList.Where(x => x.Danger4 > 0).Select(x => x.CurrencyName).Distinct();
            var currency5 = dangerList.Where(x => x.Danger5 > 0).Select(x => x.CurrencyName).Distinct();

            this.DropdownList.DangerList.Danger1.DangerItems = dangerList.Where(x => x.Danger1 > 0).Select(x => x.Danger1).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Original " + x })
                .OrderBy(x => x.Danger);

            this.DropdownList.DangerList.Danger2.DangerItems = dangerList.Where(x => x.Danger2 > 0).Select(x => x.Danger2).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "ABI " + x })
                .OrderBy(x => x.Danger);

            this.DropdownList.DangerList.Danger3.DangerItems = dangerList.Where(x => x.Danger3 > 0).Select(x => x.Danger3).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Other Sports " + x })
                .OrderBy(x => x.Danger);

            this.DropdownList.DangerList.Danger4.DangerItems = dangerList.Where(x => x.Danger4 > 0).Select(x => x.Danger4).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Basketball " + x })
                .OrderBy(x => x.Danger);

            this.DropdownList.DangerList.Danger5.DangerItems = dangerList.Where(x => x.Danger5 > 0).Select(x => x.Danger5).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "E-Sports " + x })
                .OrderBy(x => x.Danger);

            if (DropdownFilterList.Any(x => x.DangerToString == "-"))
            {
                this.DropdownList.DangerList.NoneDanger.DangerItems = new List<DangerItemViewModel> { new DangerItemViewModel { Danger = 0, DangerName = "-" } };
            }
            #endregion
                       
        }
    }
}
