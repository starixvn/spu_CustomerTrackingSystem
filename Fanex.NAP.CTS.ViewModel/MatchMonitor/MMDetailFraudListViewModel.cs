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

    public class MMDetailFraudListViewModel// : MMDetailFilterViewModel
    {
        public MMDetailFraudListViewModel() : base()
        {
            this.CTSCustIDList = string.Empty;
            this.HomeMatchItemList = new List<MatchMonitorDetailItemViewModel>();
            this.AwayMatchItemList = new List<MatchMonitorDetailItemViewModel>();
            //this.ItemList = new List<MatchMonitorDetailItemViewModel>();
            //this.DropdownList = new MMDetailFilterDropdownListViewModel();
            //this.DropdownFilterList = new List<MMFilterItemViewModel>();
        }

        private MMDetailFraudListViewModel(MMDetailFraudListViewModel matchMonitorDetailList) : base()
        {
            this.LastSequenceID = matchMonitorDetailList.LastSequenceID;
            this.CTSCustIDList = matchMonitorDetailList.CTSCustIDList;
            this.HomeMatchItemList = matchMonitorDetailList.HomeMatchItemList.ToList();
            this.AwayMatchItemList = matchMonitorDetailList.AwayMatchItemList.ToList();
            //this.ItemList = new List<MatchMonitorDetailItemViewModel>();
            //this.DropdownList = matchMonitorDetailList.DropdownList;
            //this.DropdownFilterList = matchMonitorDetailList.DropdownFilterList;
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

            //var mdItemList = new List<MatchMonitorDetailItemViewModel>();
            //mdItemList.AddRange(HomeMatchItemList);
            //mdItemList.AddRange(AwayMatchItemList);

            //RecordsTotal = (HomeMatchItemList?.Count() ?? 0) + (AwayMatchItemList?.Count() ?? 0);

            //ScoreList = mdItemList.Select(x => x.Score).Distinct().OrderBy(x => x).ToList();

            //CurrencyList = mdItemList.Select(x =>
            //{
            //    return new { CurrencyId = string.IsNullOrEmpty(x.CurrencyName) ? -1 : x.CurrencyId, CurrencyName = string.IsNullOrEmpty(x.CurrencyName) ? "-" : x.CurrencyName };
            //}).Distinct(x => x.CurrencyId)
            //.OrderBy(x => x.CurrencyName)
            //.Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName)).ToList();

            //HDPList = mdItemList.Select(x => x.BallHeadHDP()).Distinct().OrderBy(x => x).ToList();

            //BetTeamList = mdItemList.Select(x => x.BallHeadBetTeam()).Distinct().OrderBy(x => x).ToList();            

            //BindReasons(mdItemList.Select(x => x.ReasonList).Distinct());

            //var agroup = ReasonGroupIDDic.Where(x => ReasonList.Contains(x.Key)).SelectMany(x => x.Value).Distinct();

            //GroupList = GroupList?.Where(x => x.GroupID != null ? agroup.Contains(x.GroupID ?? 0) : false).ToList();

            //TicketStatusList = mdItemList.Select(x => x.TicketStatus).Distinct().ToList();
        }

        public long RecordsTotal { get; set; }

        public bool IsLive { get; set; }

        public int BetTypeID { get; set; }

        public long? LastSequenceID { get; set; }

        public string CTSCustIDList { get; set; }

        public bool IsSingleGrid { get; set; }

        public List<MatchMonitorDetailItemViewModel> HomeMatchItemList { get; set; }

        public List<MatchMonitorDetailItemViewModel> AwayMatchItemList { get; set; }

        //public List<MatchMonitorDetailItemViewModel> ItemList { get; set; }

        public MMDetailFraudListViewModel Clone() => new MMDetailFraudListViewModel(this);

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

        public void BindReasons(IEnumerable<string> rawReasons, MMDetailFilterViewModel filter)
        {
            var reasons = new List<string>();

            foreach (var reason in rawReasons)
            {
                if (!string.IsNullOrEmpty(reason))
                    reasons.AddRange(reason.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            filter.ReasonList = reasons.Distinct().ToList();
            filter.ReasonInfos = string.Join(", ", filter.ReasonList);
            filter.ReasonGroupIDDic = new Dictionary<string, List<int>>();

            var reasonGroupIDList = new List<int>();

            foreach (var reason in filter.ReasonList)
            {
                var reasonGroupIDHs = this.HomeMatchItemList.Where(x => x.ReasonList.Contains(reason)).Select(x => x.ADetectGroupID);
                var reasonGroupIDAs = this.AwayMatchItemList.Where(x => x.ReasonList.Contains(reason)).Select(x => x.ADetectGroupID);

                reasonGroupIDList.Clear();
                reasonGroupIDList.AddRange(reasonGroupIDHs);
                reasonGroupIDList.AddRange(reasonGroupIDAs);

                filter.ReasonGroupIDDic.Add(reason, reasonGroupIDList.Distinct().ToList());
            }

        }

        //public MMDetailFilterViewModel BindDropdownFilterList(IEnumerable<MatchMonitorDetailItemViewModel> mdItemList, MMDetailFilterViewModel filter)
        public MMDetailFilterViewModel BindDropdownFilterList(MMDetailFilterViewModel filter)
        {
            if (filter.DropdownList == null)
                filter.DropdownList = new MMDetailFilterDropdownListViewModel();

            //var hashSet = new HashSet<string>();

            //foreach (var item in mdItemList)
            //{
            //    var reasonsList = item.ReasonList.Split(',');
            //    foreach (var reason in reasonsList)
            //    {
            //        hashSet.Add(JsonConvert.SerializeObject(new MMFilterItemViewModel
            //        {
            //            ADetectGroup = item.ADetectGroup,
            //            ADetectGroupID = item.ADetectGroupID,
            //            CurrencyId = item.CurrencyId,
            //            CurrencyName = item.CurrencyName,
            //            Danger1 = item.Danger1,
            //            Danger2 = item.Danger2,
            //            Danger3 = item.Danger3,
            //            Danger4 = item.Danger4,
            //            Danger5 = item.Danger5,
            //            Score = item.Score,
            //            SubscriberType = item.SubscriberType,
            //            CC = item.CustomerClass,
            //            ReasonItem = reason,
            //            BallHeadHDP = item.BallHeadHDP(),
            //            BallHeadBetTeam = item.BallHeadBetTeam(),
            //            TicketStatus = item.TicketStatus
            //        }));
            //    }
            //}

            //filter.DropdownFilterList = hashSet.Select(JsonConvert.DeserializeObject<MMFilterItemViewModel>);

            filter.CurrencyList = filter.DropdownFilterList.Select(x =>
            {
                return new { CurrencyId = string.IsNullOrEmpty(x.CurrencyName) ? -1 : x.CurrencyId, CurrencyName = string.IsNullOrEmpty(x.CurrencyName) ? "-" : x.CurrencyName };
            }).Distinct(x => x.CurrencyId)
                  .OrderBy(x => x.CurrencyName)
                  .Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName))
                  .ToList();

            filter.HDPList = filter.DropdownFilterList.Select(x => x.BallHeadHDP).Distinct().OrderBy(x => x).ToList();

            filter.BetTeamList = filter.OrderBetTeams(filter.DropdownFilterList.Select(x => x.BallHeadBetTeam).Distinct());

            filter.ScoreList = filter.DropdownFilterList.Select(x => x.Score).Distinct(x => x).OrderBy(x => x).ToList();

            filter.GroupList = filter.DropdownFilterList.Select(x => new GroupInfoViewModel
            {
                GroupID = x.ADetectGroupID,
                GroupName = x.ADetectGroup
            }).Distinct(x => x.GroupID).OrderBy(x => x.GroupID).ToList();

            if (filter.DropdownFilterList.Any())
                filter.MGroupList = new List<string>() { "-" };

            var subscriberTypeList = filter.DropdownFilterList.Select(x => x.SubscriberType).Distinct().ToList();

            if (subscriberTypeList.Count() == 2)
                subscriberTypeList.Insert(0, "A");

            filter.DropdownList.SubscriberTypeList = subscriberTypeList;

            filter.DropdownList.CCList = filter.DropdownFilterList.Select(x => x.CC).Distinct();

            filter.DropdownList.TicketStatusList = filter.DropdownFilterList.Select(x => x.TicketStatus).Distinct()
                            .Select(x => new TicketStatusViewModel
                            {
                                TicketStatus = x,
                                TicketStatusName = Enum.GetName(typeof(StatusTicket), x)
                            }).OrderBy(x => x.StatusOrderIdx);

            #region Danger List
            var dangerList = filter.DropdownFilterList.Where(x => !string.IsNullOrEmpty(x.DangerToString));
            var currency1 = dangerList.Where(x => x.Danger1 > 0).Select(x => x.CurrencyName).Distinct();
            var currency2 = dangerList.Where(x => x.Danger2 > 0).Select(x => x.CurrencyName).Distinct();
            var currency3 = dangerList.Where(x => x.Danger3 > 0).Select(x => x.CurrencyName).Distinct();
            var currency4 = dangerList.Where(x => x.Danger4 > 0).Select(x => x.CurrencyName).Distinct();
            var currency5 = dangerList.Where(x => x.Danger5 > 0).Select(x => x.CurrencyName).Distinct();

            filter.DropdownList.DangerList.Danger1.DangerItems = dangerList.Where(x => x.Danger1 > 0).Select(x => x.Danger1).Distinct()
               .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Original " + x })
               .OrderBy(x => x.Danger);

            filter.DropdownList.DangerList.Danger2.DangerItems = dangerList.Where(x => x.Danger2 > 0).Select(x => x.Danger2).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "ABI " + x })
                .OrderBy(x => x.Danger);

            filter.DropdownList.DangerList.Danger3.DangerItems = dangerList.Where(x => x.Danger3 > 0).Select(x => x.Danger3).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Other Sports " + x })
                .OrderBy(x => x.Danger);

            filter.DropdownList.DangerList.Danger4.DangerItems = dangerList.Where(x => x.Danger4 > 0).Select(x => x.Danger4).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Basketball " + x })
                .OrderBy(x => x.Danger);

            filter.DropdownList.DangerList.Danger5.DangerItems = dangerList.Where(x => x.Danger5 > 0).Select(x => x.Danger5).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "E-Sports " + x })
                .OrderBy(x => x.Danger);

            if (dangerList.Any(x => x.DangerToString == "-"))
            {
                filter.DropdownList.DangerList.NoneDanger.DangerItems = new List<DangerItemViewModel> { new DangerItemViewModel { Danger = 0, DangerName = "-" } };
            }
            #endregion

            return filter;
        }

        public MMDetailFilterViewModel BuildFilterByFraudTickets(IEnumerable<MMDetailFilterTicketInfoModel> tiItemList, bool liveIndicator)
        {
            var filter = new MMDetailFilterViewModel();

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
                    ReasonItem = item.SuspiciousReason,
                    ADetectGroup = item.SuspiciousGroupId.ToString(),
                    ADetectGroupID = item.SuspiciousGroupId,
                    TicketStatus = item.TicketStatus
                }));
            }

            filter.DropdownFilterList = hashSet.Select(JsonConvert.DeserializeObject<MMFilterItemViewModel>);

            filter.RecordsTotal = (HomeMatchItemList?.Count() ?? 0) + (AwayMatchItemList?.Count() ?? 0);

            //filter.ScoreList = mdItemList.Select(x => x.Score).Distinct().OrderBy(x => x).ToList();

            //filter.CurrencyList = mdItemList.Select(x =>
            //{
            //    return new { CurrencyId = string.IsNullOrEmpty(x.CurrencyName) ? -1 : x.CurrencyId, CurrencyName = string.IsNullOrEmpty(x.CurrencyName) ? "-" : x.CurrencyName };
            //}).Distinct(x => x.CurrencyId)
            //.OrderBy(x => x.CurrencyName)
            //.Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName)).ToList();

            //filter.HDPList = mdItemList.Select(x => x.BallHeadHDP()).Distinct().OrderBy(x => x).ToList();

            //filter.BetTeamList = mdItemList.Select(x => x.BallHeadBetTeam()).Distinct().OrderBy(x => x).ToList();

            //BindReasons(mdItemList.Select(x => x.ReasonList).Distinct(), filter);

            //var agroup = filter.ReasonGroupIDDic.Where(x => filter.ReasonList.Contains(x.Key)).SelectMany(x => x.Value).Distinct();

            //filter.GroupList = filter.GroupList?.Where(x => x.GroupID != null ? agroup.Contains(x.GroupID ?? 0) : false).ToList();

            //filter.TicketStatusList = mdItemList.Select(x => x.TicketStatus).Distinct().ToList();

            filter = this.BindDropdownFilterList(filter);

            return filter;
        }

    }
}