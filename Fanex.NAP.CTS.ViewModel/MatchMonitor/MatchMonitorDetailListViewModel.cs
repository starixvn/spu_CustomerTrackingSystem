namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn;
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
            this.DropdownList = new MMDetailFilterDropdownListViewModel();
        }

        private MatchMonitorDetailListViewModel(MatchMonitorDetailListViewModel matchMonitorDetailList) : base(matchMonitorDetailList)
        {
            this.LastSequenceID = matchMonitorDetailList.LastSequenceID;
            this.CTSCustIDList = matchMonitorDetailList.CTSCustIDList;
            this.HomeMatchItemList = matchMonitorDetailList.HomeMatchItemList.ToList();
            this.AwayMatchItemList = matchMonitorDetailList.AwayMatchItemList.ToList();
            this.DropdownList = matchMonitorDetailList.DropdownList;
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
            }).Distinct()
            .OrderBy(x => x.CurrencyName)
            .Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName)).ToList();

            HDPList = mdItemList.Select(x => x.BallHeadHDP()).Distinct().OrderBy(x => x).ToList();

            RecordsTotal = (HomeMatchItemList?.Count() ?? 0) + (AwayMatchItemList?.Count() ?? 0);

            BindReasons(mdItemList.Select(x => x.ReasonList).Distinct());

            var agroup = ReasonGroupIDDic.Where(x => ReasonList.Contains(x.Key)).SelectMany(x => x.Value).Distinct();

            GroupList = GroupList?.Where(x => x.GroupID != null ? agroup.Contains(x.GroupID ?? 0) : false).ToList();
        }       

        public bool IsLive { get; set; }

        public int BetTypeID { get; set; }        

        public long? LastSequenceID { get; set; }

        public string CTSCustIDList { get; set; }

        public List<MatchMonitorDetailItemViewModel> HomeMatchItemList { get; set; }

        public List<MatchMonitorDetailItemViewModel> AwayMatchItemList { get; set; }

        public MMDetailFilterDropdownListViewModel DropdownList { get; set; }

        public new MatchMonitorDetailListViewModel Clone() => new MatchMonitorDetailListViewModel(this);

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

        public void BindDropdownList(IEnumerable<MatchMonitorDetailItemViewModel> mdItemList)
        {
            var tasks = new Task[]
                {
                    Task.Run(() =>
                    {
                        #region Score - Currency
                        DropdownList.ScoreCurrencyList = mdItemList.GroupBy(x => x.Score)
                            .Select(g => new ScoreCurrencyViewModel
                            {
                                Score = g.Key,
                                CurrencyItems = g.Select(x =>
                                {
                                    var isNotValid = string.IsNullOrEmpty(x.CurrencyName) || x.CurrencyId == 0;
                                    return new ScoreCurrencyItemViewModel { CurrencyId = isNotValid ? -1 : x.CurrencyId, CurrencyName = isNotValid ? "-" : x.CurrencyName };
                                }).Distinct(x => x.CurrencyId)
                            });
                        #endregion
                    }),
                    Task.Run(() =>
                    {
                        #region SubscriberType - Currency
                         DropdownList.SubscriberTypeCurrencyList = mdItemList.GroupBy(x => x.SubscriberType)
                            .Select(g => new SubscriberTypeCurrencyViewModel
                            {
                                SubscriberType = g.Key,
                                CurrencyItems = g.Select(x=>
                                {
                                    var isNotValid = string.IsNullOrEmpty(x.CurrencyName) || x.CurrencyId == 0;
                                    return new SubscriberTypeCurrencyItemViewModel { CurrencyId = isNotValid ? -1 : x.CurrencyId, CurrencyName = isNotValid ? "-" : x.CurrencyName };
                                }).Distinct(x => x.CurrencyId)
                            });
                        #endregion
                    }),
                    Task.Run(() =>
                    {
                        #region Currency - Danger                                            
                        var dangerList = new List<DangerCurrencyViewModel>();
                       dangerList.AddRange(mdItemList.Where(x=> x.Danger1 > 0).GroupBy(x=> new { x.Danger1 })
                            .Select(g => new DangerCurrencyViewModel {
                                DangerType = 1,
                                DangerValue = g.Key.Danger1,
                                CurrencyItems = g.Select(x=> new DangerCurrencyItemViewModel { CurrencyId = x.CurrencyId, CurrencyName = x.CurrencyName}).Distinct(x=>x.CurrencyId)
                            }));
                        dangerList.AddRange(mdItemList.Where(x=> x.Danger2 > 0).GroupBy(x=> new { x.Danger2 })
                            .Select(g => new DangerCurrencyViewModel {
                                DangerType = 2,
                                DangerValue = g.Key.Danger2,
                                CurrencyItems = g.Select(x=> new DangerCurrencyItemViewModel { CurrencyId = x.CurrencyId, CurrencyName = x.CurrencyName}).Distinct(x=>x.CurrencyId)
                            }));
                        dangerList.AddRange(mdItemList.Where(x=> x.Danger3 > 0).GroupBy(x=> new { x.Danger3 })
                            .Select(g => new DangerCurrencyViewModel {
                                DangerType = 3,
                                DangerValue = g.Key.Danger3,
                                CurrencyItems = g.Select(x=> new DangerCurrencyItemViewModel { CurrencyId = x.CurrencyId, CurrencyName = x.CurrencyName}).Distinct(x=>x.CurrencyId)
                            }));
                        dangerList.AddRange(mdItemList.Where(x=> x.Danger4 > 0).GroupBy(x=> new { x.Danger4 })
                            .Select(g => new DangerCurrencyViewModel {
                                DangerType = 4,
                                DangerValue = g.Key.Danger4,
                                CurrencyItems = g.Select(x=> new DangerCurrencyItemViewModel { CurrencyId = x.CurrencyId, CurrencyName = x.CurrencyName}).Distinct(x=>x.CurrencyId)
                            }));
                        dangerList.AddRange(mdItemList.Where(x=> x.Danger5 > 0).GroupBy(x=> new { x.Danger5 })
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
                        #region Currency - Reason
                        DropdownList.CurrencyReasonList = mdItemList.GroupBy(x => x.CurrencyId)
                            .Select(g => new CurrencyReasonViewModel
                            {
                                CurrencyId = g.Key,
                                ReasonItems = g.Select(x => new CurrencyReasonItemViewModel { ReasonList = x.ReasonList }).Distinct(x=>x.ReasonList)
                            });
                        #endregion
                    }),                     
                    Task.Run(() =>
                    {
                        #region Reason - AGroup
                        DropdownList.ReasonAGroupList = mdItemList.GroupBy(x => x.ReasonList)
                            .Select(g => new ReasonAGroupViewModel
                            {
                                Reason = g.Key,
                                AGroupItems = g.Select(x => new ReasonAGroupItemViewModel { AGroupID = x.ADetectGroupID }).Distinct()
                            });
                        #endregion
                    }),
                    Task.Run(() =>
                    {
                        #region Danger                       
                        var dangerList = mdItemList.Where(x=> !string.IsNullOrEmpty(x.DangerToString));                       
                        var currency1 = dangerList.Where(x=>x.Danger1 > 0).Select(x=>x.CurrencyName).Distinct();
                        var currency2 = dangerList.Where(x=>x.Danger2 > 0).Select(x=>x.CurrencyName).Distinct();
                        var currency3 = dangerList.Where(x=>x.Danger3 > 0).Select(x=>x.CurrencyName).Distinct();
                        var currency4 = dangerList.Where(x=>x.Danger4 > 0).Select(x=>x.CurrencyName).Distinct();
                        var currency5 = dangerList.Where(x=>x.Danger5 > 0).Select(x=>x.CurrencyName).Distinct();

                         DropdownList.Danger.Danger1.DangerItems = dangerList.Where(x=>x.Danger1 > 0).Select(x=>x.Danger1).Distinct()
                            .Select(x=> new DangerItemViewModel{ Danger = x, DangerName = "Original " + x })
                            .OrderBy(x=>x.Danger);

                        DropdownList.Danger.Danger2.DangerItems = dangerList.Where(x=>x.Danger2 > 0).Select(x=>x.Danger2).Distinct()
                            .Select(x=> new DangerItemViewModel{ Danger = x, DangerName = "ABI " + x })
                            .OrderBy(x=>x.Danger);
                        
                        DropdownList.Danger.Danger3.DangerItems = dangerList.Where(x=>x.Danger3 > 0).Select(x=>x.Danger3).Distinct()
                            .Select(x=> new DangerItemViewModel{ Danger = x, DangerName = "Other Sports " + x })
                            .OrderBy(x=>x.Danger);

                        DropdownList.Danger.Danger4.DangerItems = dangerList.Where(x=>x.Danger4 > 0).Select(x=>x.Danger4).Distinct()
                            .Select(x=> new DangerItemViewModel{ Danger = x, DangerName = "Basketball " + x })
                            .OrderBy(x=>x.Danger);

                        DropdownList.Danger.Danger5.DangerItems = dangerList.Where(x=>x.Danger5 > 0).Select(x=>x.Danger5).Distinct()
                            .Select(x=> new DangerItemViewModel{ Danger = x, DangerName = "ESports " + x })
                            .OrderBy(x=>x.Danger);
                       
                        if (mdItemList.Any(x=>string.IsNullOrEmpty(x.DangerToString)))
                        {
                             DropdownList.Danger.NoneDanger.DangerItems = new List<DangerItemViewModel> { new DangerItemViewModel {  Danger = 0, DangerName = "-"} };
                        }
                        #endregion
                    }),
                    Task.Run(() =>
                    {
                        #region Ticket Status
                        DropdownList.TicketStatusList = mdItemList.Select(x=>x.TicketStatus).Distinct()
                            .Select(x=> new TicketStatusViewModel {
                                 TicketStatus = x,
                                 TicketStatusName = Enum.GetName(typeof(TicketStatus), x)
                            });
                        #endregion
                    })
                };

            Task.WhenAll(tasks).Wait();               
        }       
    }
}