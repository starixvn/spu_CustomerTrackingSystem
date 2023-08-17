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
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MMDetailFilterViewModel
    {
        public MMDetailFilterViewModel()
        {
            this.ReasonInfos = string.Empty;
        }

        public MMDetailFilterViewModel(MMTicketDetailInfoModel mMTicket
            , MMDetailFilterViewModel fraudFilter
            , string site
            , bool isLiveIndicator)
        {
            var isFilterBySite = true;
            var isFilterLicensee = false;

            switch (site)
            {
                case MMSites.All:
                    {
                        this.RecordsTotal = mMTicket.TotalTicketAll;
                        isFilterBySite = false; break;
                    }
                case MMSites.Credit:
                    {
                        this.RecordsTotal = mMTicket.TotalTicketCredit; break;
                    }
                case MMSites.Licensee:
                    {
                        this.RecordsTotal = mMTicket.TotalTicketLicensee;
                        isFilterLicensee = true; break;
                    }
            }

            this.MaxSequenceId = mMTicket.MaxSequenceId;

            var mtFilterBySite = mMTicket.TicketInfoList;

            if (mtFilterBySite.Any())
            {
                if (isFilterBySite)
                {
                    mtFilterBySite = mtFilterBySite.Where(x => x.IsLicensee == isFilterLicensee);
                }

                this.CurrencyList = mtFilterBySite.Select(x =>
                {
                    return new { CurrencyId = string.IsNullOrEmpty(x.CurrencyName) ? -1 : x.CurrencyId, CurrencyName = string.IsNullOrEmpty(x.CurrencyName) ? "-" : x.CurrencyName };
                }).Distinct(x => x.CurrencyId)
                    .OrderBy(x => x.CurrencyName)
                    .Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName))
                    .ToList();

                this.HDPList = mtFilterBySite.Select(x => x.Hdp).Distinct().OrderBy(x => x).ToList();

                this.BetTeamList = mtFilterBySite.Select(x => x.BetTeam).Distinct().OrderBy(x => x).ToList();

                this.ScoreList = mtFilterBySite.Select(x =>
                {
                    return isLiveIndicator ? string.Format($"{x.LiveHomeScore} - {x.LiveAwayScore}") : "-";
                }).Distinct(x => x).OrderBy(x => x).ToList();

                //this.SubscriberTypeList = mtFilterBySite.Select(x => { return x.IsLicensee ? "L" : "C"; }).Distinct().ToList();

                var groupList = fraudFilter.GroupList?.ToList() ?? new List<GroupInfoViewModel>();
                //var reasonGroupIDDic = fraudFilter?.ReasonGroupIDDic ?? new Dictionary<string, List<int>>();

                // Group List
                this.GroupList = fraudFilter.GroupList?.ToList() ?? new List<GroupInfoViewModel>();

                // Reason List              
                this.ReasonList = fraudFilter.ReasonList?.ToList() ?? new List<string>();

                if (fraudFilter.RecordsTotal < RecordsTotal)
                {
                    if (!this.GroupList.Any(x => x.GroupName == "-"))
                        this.GroupList.Insert(0, new GroupInfoViewModel { GroupID = 0, GroupName = "-" });
                    if (!this.ReasonList.Any(x => x == "-"))
                        this.ReasonList.Insert(0, "-");

                    //if (!reasonList.Any(x => x == "-")) reasonList.Insert(0, "-");
                    //if (!reasonGroupIDDic.Any(x => x.Key == "-")) reasonGroupIDDic.Add("-", new List<int>() { 0 });
                    //if (!groupList.Any(x => x.GroupName == "-")) groupList.Insert(0, new GroupInfoViewModel
                    //{
                    //    GroupID = 0,
                    //    GroupName = "-"
                    //});
                }

                //ReasonInfos = string.Join(", ", ReasonList.Where(x => !x.Contains("-")));
                //ReasonGroupIDDic = reasonGroupIDDic.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

                //var agroup = ReasonGroupIDDic.Where(x => ReasonList.Contains(x.Key)).SelectMany(x => x.Value).Distinct();

                // Group List
                //this.GroupList = groupList?.Where(x => x.GroupID != null ? agroup.Contains(x.GroupID ?? 0) : false).ToList();
            }
            else
            {
                this.SubscriberTypeList = new List<string>();
                this.CurrencyList = new List<KeyValuePair<int, string>>();
                this.HDPList = new List<decimal>();
                this.BetTeamList = new List<string>();
                this.ScoreList = new List<string>();
                this.ReasonList = new List<string>();
                this.GroupList = new List<GroupInfoViewModel>();
            }

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

        public List<MMFilterItemViewModel> DropdownFilterList { get; set; }

        public MMDetailFilterViewModel Clone() => new MMDetailFilterViewModel(this);

        public bool IsValidFilterTicketLatest(MMDetailFilterViewModel filterLastest)
        {
            if (IsFilter)
            {
                var count = 0;

                if (filterLastest.HDPList != null && this.HDPList != null && !filterLastest.HDPList.Except(HDPList).Any())
                {
                    HDPList = null;
                    count++;
                }

                if (filterLastest.BetTeamList != null && this.BetTeamList != null && !filterLastest.BetTeamList.Except(BetTeamList).Any())
                {
                    BetTeamList = null;
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

                IsFilter = count < 8;
            }

            return IsFilter;
        }

        //public bool IsOnlyFraudTicket()
        //{           
        //    return (!ReasonList?.Contains("-") ?? false) || !((AGroupList?.Contains("-") ?? true) || (AGroupList?.Contains("0") ?? true));
        //}

        //private bool FilterBySite(MMdetailFilterInforTypeBaseModel x, string site)
        //{
        //    if (site == MMSites.All) return true;

        //    if (site == MMSites.Credit) return x.IsCredit == 1;

        //    if (site == MMSites.Licensee) return x.IsLicensee == 1;

        //    return false;
        //}       

        public void BindDropdownListByMainTicketInfo(IEnumerable<MMDetailFilterTicketInfoModel> tiItemList, bool liveIndicator)
        {
            if (this.DropdownList == null)
            {
                this.DropdownList = new MMDetailFilterDropdownListViewModel();
            }

            var tiItemViewList = tiItemList.Select(x => new MMDetailFilterTicketInfoViewModel(x, liveIndicator));

            var tasks = new Task[]
            {
                Task.Run(() =>
                {
                    #region SubscriberType - Currency                         
                        var subscriberTypeCurrency = new List<SubscriberTypeCurrencyViewModel>();

                        subscriberTypeCurrency.AddRange(tiItemViewList.GroupBy(x => x.IsLicensee)
                            .Select(g => new SubscriberTypeCurrencyViewModel
                            {
                                SubscriberType = g.Key == true ? "L" : "C",
                                CurrencyItems = g.Select(x=>
                                {
                                    var isNotValid = string.IsNullOrEmpty(x.CurrencyName) || x.CurrencyId == 0;
                                    return new SubscriberTypeCurrencyItemViewModel { CurrencyId = isNotValid ? -1 : x.CurrencyId, CurrencyName = isNotValid ? "-" : x.CurrencyName };
                                }).Distinct(x => x.CurrencyId)
                            }));

                        if (subscriberTypeCurrency.Count == 2)
                        {
                            subscriberTypeCurrency.Insert(0, new SubscriberTypeCurrencyViewModel{
                                 SubscriberType = "A",
                                 CurrencyItems = tiItemList.Select(x=>
                                    {
                                        var isNotValid = string.IsNullOrEmpty(x.CurrencyName) || x.CurrencyId == 0;
                                        return new SubscriberTypeCurrencyItemViewModel { CurrencyId = isNotValid ? -1 : x.CurrencyId, CurrencyName = isNotValid ? "-" : x.CurrencyName };
                                    }).Distinct(x => x.CurrencyId)
                            });
                        }

                        DropdownList.SubscriberTypeCurrencyList = subscriberTypeCurrency;

                    #endregion
                }),
                Task.Run(() =>
                {
                    #region  Danger - Currency                        
                    var dangerList = new List<DangerCurrencyViewModel>();

                    dangerList.AddRange(tiItemViewList.Where(x=> x.Danger1 == 0 && x.Danger2 == 0 && x.Danger3 == 0 && x.Danger4 == 0 && x.Danger5 == 0).GroupBy(x=>x.Danger1)
                        .Select(g => new DangerCurrencyViewModel {
                                DangerType = 0,
                                DangerValue = 0,
                                CurrencyItems = g.Select(x=> new DangerCurrencyItemViewModel { CurrencyId = x.CurrencyId, CurrencyName = x.CurrencyName}).Distinct(x=>x.CurrencyId)
                            }));
                    dangerList.AddRange(tiItemViewList.Where(x=> x.Danger1 > 0).GroupBy(x=> new { x.Danger1 })
                        .Select(g => new DangerCurrencyViewModel {
                            DangerType = 1,
                            DangerValue = g.Key.Danger1,
                            CurrencyItems = g.Select(x=> new DangerCurrencyItemViewModel { CurrencyId = x.CurrencyId, CurrencyName = x.CurrencyName}).Distinct(x=>x.CurrencyId)
                        }));
                    dangerList.AddRange(tiItemViewList.Where(x=> x.Danger2 > 0).GroupBy(x=> new { x.Danger2 })
                        .Select(g => new DangerCurrencyViewModel {
                            DangerType = 2,
                            DangerValue = g.Key.Danger2,
                            CurrencyItems = g.Select(x=> new DangerCurrencyItemViewModel { CurrencyId = x.CurrencyId, CurrencyName = x.CurrencyName}).Distinct(x=>x.CurrencyId)
                        }));
                    dangerList.AddRange(tiItemViewList.Where(x=> x.Danger3 > 0).GroupBy(x=> new { x.Danger3 })
                        .Select(g => new DangerCurrencyViewModel {
                            DangerType = 3,
                            DangerValue = g.Key.Danger3,
                            CurrencyItems = g.Select(x=> new DangerCurrencyItemViewModel { CurrencyId = x.CurrencyId, CurrencyName = x.CurrencyName}).Distinct(x=>x.CurrencyId)
                        }));
                    dangerList.AddRange(tiItemViewList.Where(x=> x.Danger4 > 0).GroupBy(x=> new { x.Danger4 })
                        .Select(g => new DangerCurrencyViewModel {
                            DangerType = 4,
                            DangerValue = g.Key.Danger4,
                            CurrencyItems = g.Select(x=> new DangerCurrencyItemViewModel { CurrencyId = x.CurrencyId, CurrencyName = x.CurrencyName}).Distinct(x=>x.CurrencyId)
                        }));
                    dangerList.AddRange(tiItemViewList.Where(x=> x.Danger5 > 0).GroupBy(x=> new { x.Danger5 })
                        .Select(g => new DangerCurrencyViewModel {
                            DangerType = 5,
                            DangerValue = g.Key.Danger5,
                            CurrencyItems = g.Select(x=> new DangerCurrencyItemViewModel { CurrencyId = x.CurrencyId, CurrencyName = x.CurrencyName}).Distinct(x=>x.CurrencyId)
                        }));

                    DropdownList.DangerCurrencyList = dangerList;
                    #endregion
                }),
                Task.Run(() =>
                {
                    #region Score - Currency
                    this.DropdownList.ScoreCurrencyList = tiItemViewList.GroupBy(x => x.Score)
                            .Select(g => new ScoreCurrencyViewModel
                            {
                                Score = g.Key,
                                CurrencyItems = g.Select(x =>
                                {
                                    var isNotValid = string.IsNullOrEmpty(x.CurrencyName) || x.CurrencyId == 0;
                                    return new ScoreCurrencyItemViewModel
                                    {
                                        CurrencyId = isNotValid ? -1 : x.CurrencyId,
                                        CurrencyName = isNotValid ? "-" : x.CurrencyName,
                                    };
                                }).Distinct(x => x.CurrencyId)
                            });
                    #endregion
                }),
                Task.Run(() =>
                {
                    #region Currency - Reason
                   DropdownList.CurrencyReasonList = tiItemViewList.GroupBy(x => new { x.CurrencyId })
                            .Select(g => new CurrencyReasonViewModel
                            {
                                CurrencyId = g.Key.CurrencyId,
                                ReasonItems = g.Select(x => new CurrencyReasonItemViewModel { ReasonList = string.Empty}).Distinct(x=>x.ReasonList)
                            });
                    #endregion
                }),
                Task.Run(() =>
                {
                    #region Danger                       
                    var dangerList = tiItemViewList.Where(x=> x.IsDanger);
                    var currency1 = dangerList.Where(x=>x.Danger1 > 0).Select(x=>x.CurrencyName).Distinct();
                    var currency2 = dangerList.Where(x=>x.Danger2 > 0).Select(x=>x.CurrencyName).Distinct();
                    var currency3 = dangerList.Where(x=>x.Danger3 > 0).Select(x=>x.CurrencyName).Distinct();
                    var currency4 = dangerList.Where(x=>x.Danger4 > 0).Select(x=>x.CurrencyName).Distinct();
                    var currency5 = dangerList.Where(x=>x.Danger5 > 0).Select(x=>x.CurrencyName).Distinct();

                    this.DropdownList.Danger.Danger1.DangerItems = dangerList.Where(x=>x.Danger1 > 0).Select(x=>x.Danger1).Distinct()
                        .Select(x=> new DangerItemViewModel{ Danger = x, DangerName = "Original " + x })
                        .OrderBy(x=>x.Danger);

                    this.DropdownList.Danger.Danger2.DangerItems = dangerList.Where(x=>x.Danger2 > 0).Select(x=>x.Danger2).Distinct()
                        .Select(x=> new DangerItemViewModel{ Danger = x, DangerName = "ABI " + x })
                        .OrderBy(x=>x.Danger);

                    this.DropdownList.Danger.Danger3.DangerItems = dangerList.Where(x=>x.Danger3 > 0).Select(x=>x.Danger3).Distinct()
                        .Select(x=> new DangerItemViewModel{ Danger = x, DangerName = "Other Sports " + x })
                        .OrderBy(x=>x.Danger);

                    this.DropdownList.Danger.Danger4.DangerItems = dangerList.Where(x=>x.Danger4 > 0).Select(x=>x.Danger4).Distinct()
                        .Select(x=> new DangerItemViewModel{ Danger = x, DangerName = "Basketball " + x })
                        .OrderBy(x=>x.Danger);

                    this.DropdownList.Danger.Danger5.DangerItems = dangerList.Where(x=>x.Danger5 > 0).Select(x=>x.Danger5).Distinct()
                        .Select(x=> new DangerItemViewModel{ Danger = x, DangerName = "E-Sports " + x })
                        .OrderBy(x=>x.Danger);

                    if (tiItemViewList.Any(x=>!x.IsDanger))
                    {
                            this.DropdownList.Danger.NoneDanger.DangerItems = new List<DangerItemViewModel> { new DangerItemViewModel {  Danger = 0, DangerName = "-"} };
                    }
                    #endregion
                }),
                Task.Run(() =>
                {
                    #region Ticket Status
                    this.DropdownList.TicketStatusList = tiItemViewList.Select(x=>x.TicketStatus).Distinct()
                        .Select(x=> new TicketStatusViewModel {
                                TicketStatus = x,
                                TicketStatusName = Enum.GetName(typeof(StatusTicket), x)
                        }).OrderBy(x=>x.StatusOrderIdx);
                    
                    #endregion
                })
            };

            Task.WhenAll(tasks).Wait();
        }

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
                }));
            }

            DropdownFilterList = hashSet.Select(JsonConvert.DeserializeObject<MMFilterItemViewModel>).ToList();
        }

        public void BindDropdownListByFraudTicket(MMDetailFilterViewModel fraudTicketModel)
        {
            if (this.DropdownList == null)
            {
                this.DropdownList = new MMDetailFilterDropdownListViewModel();
            }

            if (fraudTicketModel.RecordsTotal < RecordsTotal && fraudTicketModel.DropdownList.CurrencyReasonList != null)
            {
                this.DropdownList.CurrencyReasonList = new List<CurrencyReasonViewModel>() { new CurrencyReasonViewModel { CurrencyId = 0 } }.Union(fraudTicketModel.DropdownList.CurrencyReasonList);
                this.DropdownList.CurrencyAGroupList = new List<CurrencyAGroupViewModel>() { new CurrencyAGroupViewModel { CurrencyId = 0 } }.Union(fraudTicketModel.DropdownList.CurrencyAGroupList);
                this.DropdownList.ReasonAGroupList = new List<ReasonAGroupViewModel>() { new ReasonAGroupViewModel { Reason = "-" } }.Union(fraudTicketModel.DropdownList.ReasonAGroupList);
                this.DropdownList.AGrMGrBlankList = new List<AGrMGrViewModel>()
                {
                    new AGrMGrViewModel { AGr = 0, MGrItems = new List<AGrMGrItemViewModel>() { new AGrMGrItemViewModel {  MGr = "-" } }}
                }.Union(fraudTicketModel.DropdownList.AGrMGrBlankList);
            }
            else
            {
                this.DropdownList.CurrencyReasonList = fraudTicketModel.DropdownList.CurrencyReasonList;
                this.DropdownList.CurrencyAGroupList = fraudTicketModel.DropdownList.CurrencyAGroupList;
                this.DropdownList.ReasonAGroupList = fraudTicketModel.DropdownList.ReasonAGroupList;
                this.DropdownList.AGrMGrBlankList = fraudTicketModel.DropdownList.AGrMGrBlankList;
            }
        }

        public void BindDropdownFilterListByFraudTicket(MMDetailFilterViewModel fraudTicketModel)
        {
            DropdownFilterList.ForEach(item =>
            {
                var filter =
                    fraudTicketModel.DropdownFilterList
                    .Where(x => x.Score == item.Score &&
                        x.SubscriberType == item.SubscriberType &&
                        x.CurrencyId == item.CurrencyId);
                
                item.FraudADetectGroupID = filter.Select(x => x.ADetectGroupID).Distinct().ToList();
                item.FraudReasonList = filter.Select(x => x.ReasonItem).Distinct().ToList();
            });
        }
    }
}
