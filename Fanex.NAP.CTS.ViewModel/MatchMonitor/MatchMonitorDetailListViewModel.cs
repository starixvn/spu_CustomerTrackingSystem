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
    using System.Threading.Tasks;

    public class MatchMonitorDetailListViewModel : MMDetailFilterViewModel
    {
        public MatchMonitorDetailListViewModel() : base()
        {
            this.CTSCustIDList = string.Empty;
            this.HomeMatchItemList = new List<MatchMonitorDetailItemViewModel>();
            this.AwayMatchItemList = new List<MatchMonitorDetailItemViewModel>();
            //this.ItemList = new List<MatchMonitorDetailItemViewModel>();
            this.DropdownList = new MMDetailFilterDropdownListViewModel();
            this.DropdownFilterList = new List<MMFilterItemViewModel>();
        }

        private MatchMonitorDetailListViewModel(MatchMonitorDetailListViewModel matchMonitorDetailList) : base(matchMonitorDetailList)
        {
            this.LastSequenceID = matchMonitorDetailList.LastSequenceID;
            this.CTSCustIDList = matchMonitorDetailList.CTSCustIDList;
            this.HomeMatchItemList = matchMonitorDetailList.HomeMatchItemList.ToList();
            this.AwayMatchItemList = matchMonitorDetailList.AwayMatchItemList.ToList();
            //this.ItemList = new List<MatchMonitorDetailItemViewModel>();
            this.DropdownList = matchMonitorDetailList.DropdownList;
            this.DropdownFilterList = matchMonitorDetailList.DropdownFilterList;
        }

        public void FilterItemMMDetail(MMDetailFilterViewModel filter, string site)
        {
            var reasonGroupIDDic = filter.AGroupList?.Select(x => x.Contains("-") ? "0" : x);

            HomeMatchItemList = HomeMatchItemList.Where(x => x.FilterItem(filter, reasonGroupIDDic, site)).ToList();
            AwayMatchItemList = AwayMatchItemList.Where(x => x.FilterItem(filter, reasonGroupIDDic, site)).ToList();
        }
   
        public void FilterItemMMDetailBySite(string site)
        {
            HomeMatchItemList = HomeMatchItemList.Where(x => x.FilterBySite(site)).ToList();
            AwayMatchItemList = AwayMatchItemList.Where(x => x.FilterBySite(site)).ToList();

            var mdItemList = new List<MatchMonitorDetailItemViewModel>();
            mdItemList.AddRange(HomeMatchItemList);
            mdItemList.AddRange(AwayMatchItemList);

            RecordsTotal = (HomeMatchItemList?.Count() ?? 0) + (AwayMatchItemList?.Count() ?? 0);

            ScoreList = mdItemList.Select(x => x.Score).Distinct().OrderBy(x => x).ToList();

            CurrencyList = mdItemList.Select(x =>
            {
                return new { CurrencyId = string.IsNullOrEmpty(x.CurrencyName) ? -1 : x.CurrencyId, CurrencyName = string.IsNullOrEmpty(x.CurrencyName) ? "-" : x.CurrencyName };
            }).Distinct(x => x.CurrencyId)
            .OrderBy(x => x.CurrencyName)
            .Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName)).ToList();

            HDPList = mdItemList.Select(x => x.BallHeadHDP()).Distinct().OrderBy(x => x).ToList();

            BetTeamList = mdItemList.Select(x => x.BallHeadBetTeam()).Distinct().OrderBy(x => x).ToList();            

            BindReasons(mdItemList.Select(x => x.ReasonList).Distinct());

            var agroup = ReasonGroupIDDic.Where(x => ReasonList.Contains(x.Key)).SelectMany(x => x.Value).Distinct();

            GroupList = GroupList?.Where(x => x.GroupID != null ? agroup.Contains(x.GroupID ?? 0) : false).ToList();

            TicketStatusList = mdItemList.Select(x => x.TicketStatus).Distinct().ToList();
        }

        public bool IsLive { get; set; }

        public int BetTypeID { get; set; }

        public long? LastSequenceID { get; set; }

        public string CTSCustIDList { get; set; }

        public bool IsSingleGrid { get; set; }

        public List<MatchMonitorDetailItemViewModel> HomeMatchItemList { get; set; }

        public List<MatchMonitorDetailItemViewModel> AwayMatchItemList { get; set; }

        //public List<MatchMonitorDetailItemViewModel> ItemList { get; set; }

        public new MatchMonitorDetailListViewModel Clone() => new MatchMonitorDetailListViewModel(this);

        public IEnumerable<SuspiciousTransInfoModel> GetSuspiciousTransInfo()
        {
            var suspiciousTransInfos = new List<SuspiciousTransInfoModel>();

            suspiciousTransInfos.AddRange(GetSuspiciousTransInfoByTeam(this.HomeMatchItemList));
            suspiciousTransInfos.AddRange(GetSuspiciousTransInfoByTeam(this.AwayMatchItemList));

            return suspiciousTransInfos;
        }

        private IEnumerable<SuspiciousTransInfoModel> GetSuspiciousTransInfoByTeam(List<MatchMonitorDetailItemViewModel> ticketByTeam)
        {
            var suspiciousTransInfos = new List<SuspiciousTransInfoModel>();

            foreach (var ticket in ticketByTeam)
            {
                var arrReasons = ticket.ReasonList.Split(',');

                foreach (var reason in arrReasons)
                {
                    suspiciousTransInfos.Add(new SuspiciousTransInfoModel
                    {
                        TransId = ticket.TransId,
                        Reason = reason,
                        GroupId = ticket.ADetectGroupID
                    });
                }
            }

            return suspiciousTransInfos;
        }

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

        public void BindDropdownFilterList(IEnumerable<MatchMonitorDetailItemViewModel> mdItemList)
        {
            var hashSet = new HashSet<string>();

            foreach (var item in mdItemList)
            {
                var reasonsList = item.ReasonList.Split(',');
                foreach (var reason in reasonsList)
                {
                    hashSet.Add(JsonConvert.SerializeObject(new MMFilterItemViewModel
                    {
                        ADetectGroup = item.ADetectGroup,
                        ADetectGroupID = item.ADetectGroupID,
                        CurrencyId = item.CurrencyId,
                        CurrencyName = item.CurrencyName,
                        Danger1 = item.Danger1,
                        Danger2 = item.Danger2,
                        Danger3 = item.Danger3,
                        Danger4 = item.Danger4,
                        Danger5 = item.Danger5,
                        Score = item.Score,
                        SubscriberType = item.SubscriberType,
                        CC = item.CustomerClass,
                        ReasonItem = reason,
                        BallHeadHDP = item.BallHeadHDP(),
                        BallHeadBetTeam = item.BallHeadBetTeam(),
                        TicketStatus = item.TicketStatus
                    }));
                }
            }

            DropdownFilterList = hashSet.Select(JsonConvert.DeserializeObject<MMFilterItemViewModel>);                              
          
            this.CurrencyList = DropdownFilterList.Select(x =>
            {
                return new { CurrencyId = string.IsNullOrEmpty(x.CurrencyName) ? -1 : x.CurrencyId, CurrencyName = string.IsNullOrEmpty(x.CurrencyName) ? "-" : x.CurrencyName };
            }).Distinct(x => x.CurrencyId)
                  .OrderBy(x => x.CurrencyName)
                  .Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName))
                  .ToList();

            this.HDPList = DropdownFilterList.Select(x => x.BallHeadHDP).Distinct().OrderBy(x => x).ToList();

            this.BetTeamList = OrderBetTeams(DropdownFilterList.Select(x => x.BallHeadBetTeam).Distinct());

            this.ScoreList = DropdownFilterList.Select(x => x.Score).Distinct(x => x).OrderBy(x => x).ToList();

            this.GroupList = DropdownFilterList.Select(x => new GroupInfoViewModel
            {
                GroupID = x.ADetectGroupID,
                GroupName = x.ADetectGroup
            }).Distinct(x => x.GroupID).OrderBy(x => x.GroupID).ToList();

            if (DropdownFilterList.Any())
                this.MGroupList = new List<string>() { "-" };

            var subscriberTypeList = DropdownFilterList.Select(x => x.SubscriberType).Distinct().ToList();

            if (subscriberTypeList.Count() == 2)
                subscriberTypeList.Insert(0, "A");

            DropdownList.SubscriberTypeList = subscriberTypeList;

            DropdownList.CCList = DropdownFilterList.Select(x => x.CC).Distinct();

            DropdownList.TicketStatusList = DropdownFilterList.Select(x => x.TicketStatus).Distinct()
                            .Select(x => new TicketStatusViewModel
                            {
                                TicketStatus = x,
                                TicketStatusName = Enum.GetName(typeof(StatusTicket), x)
                            }).OrderBy(x => x.StatusOrderIdx);

            #region Danger List
            var dangerList = DropdownFilterList.Where(x => !string.IsNullOrEmpty(x.DangerToString));
            var currency1 = dangerList.Where(x => x.Danger1 > 0).Select(x => x.CurrencyName).Distinct();
            var currency2 = dangerList.Where(x => x.Danger2 > 0).Select(x => x.CurrencyName).Distinct();
            var currency3 = dangerList.Where(x => x.Danger3 > 0).Select(x => x.CurrencyName).Distinct();
            var currency4 = dangerList.Where(x => x.Danger4 > 0).Select(x => x.CurrencyName).Distinct();
            var currency5 = dangerList.Where(x => x.Danger5 > 0).Select(x => x.CurrencyName).Distinct();

            DropdownList.DangerList.Danger1.DangerItems = dangerList.Where(x => x.Danger1 > 0).Select(x => x.Danger1).Distinct()
               .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Original " + x })
               .OrderBy(x => x.Danger);

            DropdownList.DangerList.Danger2.DangerItems = dangerList.Where(x => x.Danger2 > 0).Select(x => x.Danger2).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "ABI " + x })
                .OrderBy(x => x.Danger);

            DropdownList.DangerList.Danger3.DangerItems = dangerList.Where(x => x.Danger3 > 0).Select(x => x.Danger3).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Other Sports " + x })
                .OrderBy(x => x.Danger);

            DropdownList.DangerList.Danger4.DangerItems = dangerList.Where(x => x.Danger4 > 0).Select(x => x.Danger4).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Basketball " + x })
                .OrderBy(x => x.Danger);

            DropdownList.DangerList.Danger5.DangerItems = dangerList.Where(x => x.Danger5 > 0).Select(x => x.Danger5).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "E-Sports " + x })
                .OrderBy(x => x.Danger);

            if (mdItemList.Any(x => x.DangerToString == "-"))
            {
                DropdownList.DangerList.NoneDanger.DangerItems = new List<DangerItemViewModel> { new DangerItemViewModel { Danger = 0, DangerName = "-" } };
            }
            #endregion
            
        }
        
    }
}