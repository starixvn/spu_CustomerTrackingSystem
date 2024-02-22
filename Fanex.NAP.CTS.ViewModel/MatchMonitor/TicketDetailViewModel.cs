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

    public class TicketDetailViewModel
    {
        public IEnumerable<TicketDetailModel> TicketDetails { get; set; }

        public MMDetailMergeGroupViewModel MMDetailMergeGroup { get; set; }

        public MMDetailResultModel MatchDetailInfo { get; set; }

        public IEnumerable<MatchMonitorDetailItemViewModel> AGroupReasonMerged { get; set; }

        public int? TotalTicket { get; set; }

        public long? MinSequenceId { get; set; }

        public DateTime? MinTransDate { get; set; }

        public string GetTransByFilterIncludeSuspicous(MMDetailFilterViewModel filter, IEnumerable<MatchMonitorDetailItemViewModel> ticketMergeGroups)
        {
            var transList = new List<FraudTransModel>();
            var mGrTranIDListFilteredDic = new Dictionary<long, MGrTranIDListFilteredModel>();

            if (!string.IsNullOrEmpty(filter.MGrTranIDListFiltered))
            {
                mGrTranIDListFilteredDic = JsonConvert.DeserializeObject<IEnumerable<MGrTranIDListFilteredModel>>(filter.MGrTranIDListFiltered).ToDictionary(x => x.TransID, x => x);
            }

            foreach (var ticket in ticketMergeGroups)
            {
                mGrTranIDListFilteredDic.TryGetValue(ticket.TransId, out var objMGr);

                var equals = filter.ReasonList.Any(y => (ticket.ReasonList ?? "-").Contains(y))
                            && filter.AGroupList.Contains(ticket.ADetectGroup);

                if (equals && objMGr != null)
                {
                    equals = filter.MGroupList.Contains(objMGr?.MGrName);
                }
                if (equals)
                {
                    if (!string.IsNullOrEmpty(ticket.ReasonIDList))
                    {
                        var arrReason = ticket.ReasonList.Split(',');

                        foreach (var item in arrReason)
                        {
                            transList.Add(new FraudTransModel
                            {
                                TransID = ticket.TransId,
                                Reason = item,
                                AGroup = ticket.ADetectGroupID,
                                MGroup = objMGr != null ? objMGr.MGr : 0
                            });
                        }
                    }
                }
            }

            transList = GetTranByFilterMGr(transList, mGrTranIDListFilteredDic);

            if (transList.Any())
            {
                return JsonConvert.SerializeObject(transList);
            }

            return string.Empty;
        }

        public string GetTransByFilterExcludeSuspicous(MMDetailFilterViewModel filter, IEnumerable<MatchMonitorDetailItemViewModel> ticketMergeGroups)
        {
            var transList = new List<FraudTransModel>();

            var mGrTranIDListFiltered = Enumerable.Empty<MGrTranIDListFilteredModel>();
            var mGrTranIDListFilteredDic = new Dictionary<long, MGrTranIDListFilteredModel>();

            if (!string.IsNullOrEmpty(filter.MGrTranIDListFiltered))
            {
                if (filter.MGroupList.Contains("-") || filter.MGroupList.Contains("0"))
                {
                    mGrTranIDListFilteredDic = JsonConvert.DeserializeObject<IEnumerable<MGrTranIDListFilteredModel>>(filter.MGrTranIDListFiltered)
                        .Where(x => !filter.MGroupList.Contains(x.MGrName)).ToDictionary(x => x.TransID, x => x);
                }
                else
                {
                    mGrTranIDListFilteredDic = JsonConvert.DeserializeObject<IEnumerable<MGrTranIDListFilteredModel>>(filter.MGrTranIDListFiltered)
                        .Where(x => filter.MGroupList.Contains(x.MGrName)).ToDictionary(x => x.TransID, x => x);
                }
            }

            foreach (var ticket in ticketMergeGroups)
            {
                mGrTranIDListFilteredDic.TryGetValue(ticket.TransId, out var objMGr);

                var equals = !filter.ReasonList.Any(y => (ticket.ReasonList ?? "-").Contains(y))
                            || !filter.AGroupList.Contains(ticket.ADetectGroup)
                            || mGrTranIDListFilteredDic.ContainsKey(ticket.TransId);

                if (equals)
                {
                    if (!string.IsNullOrEmpty(ticket.ReasonIDList))
                    {
                        var arrReason = ticket.ReasonList.Split(',');

                        foreach (var item in arrReason)
                        {
                            transList.Add(new FraudTransModel
                            {
                                TransID = ticket.TransId,
                                Reason = item,
                                AGroup = ticket.ADetectGroupID,
                                MGroup = objMGr != null ? objMGr.MGr : 0
                            });
                        }
                    }
                }
            }

            transList = GetTranByFilterMGr(transList, mGrTranIDListFilteredDic);

            if (transList.Any())
            {
                return JsonConvert.SerializeObject(transList);
            }

            return string.Empty;
        }

        public string GetTransByFilterIncludeMGr(MMDetailFilterViewModel filter)
        {
            var transList = new List<FraudTransModel>();

            var mGrTranIDListFilteredDic = new Dictionary<long, MGrTranIDListFilteredModel>();

            if (!string.IsNullOrEmpty(filter.MGrTranIDListFiltered))
            {
                var mGrTranIDListFiltered = JsonConvert.DeserializeObject<IEnumerable<MGrTranIDListFilteredModel>>(filter.MGrTranIDListFiltered)
                    .Where(x => filter.MGroupList.Contains(x.MGrName));

                mGrTranIDListFilteredDic = mGrTranIDListFiltered.ToDictionary(x => x.TransID, x => x);
            }

            transList = GetTranByFilterMGr(transList, mGrTranIDListFilteredDic);

            if (transList.Any())
            {
                return JsonConvert.SerializeObject(transList);
            }

            return string.Empty;
        }

        public string GetTransByFilterExcludeMGr(MMDetailFilterViewModel filter)
        {
            var transList = new List<FraudTransModel>();

            var mGrTranIDListFiltered = Enumerable.Empty<MGrTranIDListFilteredModel>();
            var mGrTranIDListFilteredDic = new Dictionary<long, MGrTranIDListFilteredModel>();

            if (!string.IsNullOrEmpty(filter.MGrTranIDListFiltered))
            {
                if (filter.MGroupList.Contains("-") || filter.MGroupList.Contains("0"))
                {
                    mGrTranIDListFiltered = JsonConvert.DeserializeObject<IEnumerable<MGrTranIDListFilteredModel>>(filter.MGrTranIDListFiltered)
                  .Where(x => !filter.MGroupList.Contains(x.MGrName));
                    mGrTranIDListFilteredDic = mGrTranIDListFiltered.ToDictionary(x => x.TransID, x => x);
                }
                else
                {
                    mGrTranIDListFiltered = JsonConvert.DeserializeObject<IEnumerable<MGrTranIDListFilteredModel>>(filter.MGrTranIDListFiltered)
                  .Where(x => filter.MGroupList.Contains(x.MGrName));
                    mGrTranIDListFilteredDic = mGrTranIDListFiltered.ToDictionary(x => x.TransID, x => x);
                }
            }

            transList = GetTranByFilterMGr(transList, mGrTranIDListFilteredDic);

            if (transList.Any())
            {
                return JsonConvert.SerializeObject(transList);
            }

            return string.Empty;
        }

        private List<FraudTransModel> GetTranByFilterMGr(List<FraudTransModel> transList, Dictionary<long, MGrTranIDListFilteredModel> mGrTranIDListFilteredDic)
        {
            var trandsIDs = transList.Select(x => x.TransID);
            var trandsExcept = mGrTranIDListFilteredDic.Where(d => !trandsIDs.Contains(d.Key)).Select(x => new FraudTransModel
            {
                TransID = x.Value.TransID,
                Reason = "0",
                MGroup = x.Value.MGr
            });

            transList.AddRange(trandsExcept);

            return transList;
        }

        public IEnumerable<SuspiciousTransInfoModel> GetSuspiciousTransInfo(IEnumerable<MatchMonitorDetailItemViewModel> tickets)
        {
            var suspiciousTransInfos = new List<SuspiciousTransInfoModel>();

            foreach (var ticket in tickets)
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

        public MMDetailFilterViewModel GenerateDropdownFilterListByMainTicketInfo(IEnumerable<MMDetailFilterTicketInfoModel> infoList, bool liveIndicator, int sportType)
        {
            var filter = new MMDetailFilterViewModel();

            filter.DropdownListFilterDependOn = infoList.Select(x => new DependOnFilterItemViewModel(x, liveIndicator));

            filter.CurrencyList = filter.DropdownListFilterDependOn.Select(x =>
            {
                return new { CurrencyId = string.IsNullOrEmpty(x.CurrencyName) ? -1 : x.CurrencyId, CurrencyName = string.IsNullOrEmpty(x.CurrencyName) ? "-" : x.CurrencyName };
            }).Distinct(x => x.CurrencyId)
             .OrderBy(x => x.CurrencyName)
             .Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName))
             .ToList();

            filter.ReasonList = filter.DropdownListFilterDependOn.Select(x => x.ReasonItem).Distinct().ToList();

            filter.HDPList = filter.DropdownListFilterDependOn.Select(x => x.BallHeadHDP).Distinct().OrderBy(x => x).ToList();

            filter.BetTeamList = filter.OrderByFormatScoreBetTeam(filter.DropdownListFilterDependOn.Select(x => x.BallHeadBetTeam).Distinct(), "-");

            var scoreList = filter.DropdownListFilterDependOn.Select(x => x.Score).Distinct();
            if (sportType == 1 && liveIndicator)
            {
                filter.ScoreList = filter.OrderByFormatScoreBetTeam(scoreList, " - ");
            }
            else
            {
                filter.ScoreList = scoreList.ToList();
            }

            filter.GroupList = filter.DropdownListFilterDependOn.Select(x => new GroupInfoViewModel
            {
                GroupID = x.ADetectGroupID,
                GroupName = x.ADetectGroup
            }).Distinct(x => x.GroupID).OrderBy(x => x.GroupID).ToList();

            if (filter.DropdownListFilterDependOn.Any())
                filter.MGroupList = new List<string>() { "-" };

            var subscriberTypeList = filter.DropdownListFilterDependOn.Select(x => x.SubscriberType).Distinct().ToList();

            if (subscriberTypeList.Count() == 2)
                subscriberTypeList.Insert(0, "A");

            filter.DropdownListFilter.SubscriberTypeList = subscriberTypeList;

            filter.DropdownListFilter.CCList = filter.DropdownListFilterDependOn.Select(x => x.CC).Distinct();

            filter.DropdownListFilter.TicketStatusList = filter.DropdownListFilterDependOn.Select(x => x.TicketStatus).Distinct()
                            .Select(x => new TicketStatusViewModel
                            {
                                TicketStatus = x,
                                TicketStatusName = Enum.GetName(typeof(StatusTicket), x) ?? x.ToString()
                            }).OrderBy(x => x.StatusOrderIdx);

            #region Danger List
            var dangerList = filter.DropdownListFilterDependOn.Where(x => !string.IsNullOrEmpty(x.DangerToString));
            var currency1 = dangerList.Where(x => x.Danger1 > 0).Select(x => x.CurrencyName).Distinct();
            var currency2 = dangerList.Where(x => x.Danger2 > 0).Select(x => x.CurrencyName).Distinct();
            var currency3 = dangerList.Where(x => x.Danger3 > 0).Select(x => x.CurrencyName).Distinct();
            var currency4 = dangerList.Where(x => x.Danger4 > 0).Select(x => x.CurrencyName).Distinct();
            var currency5 = dangerList.Where(x => x.Danger5 > 0).Select(x => x.CurrencyName).Distinct();

            filter.DropdownListFilter.DangerList.Danger1.DangerItems = dangerList.Where(x => x.Danger1 > 0).Select(x => x.Danger1).Distinct()
               .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Original " + x })
               .OrderBy(x => x.Danger);

            filter.DropdownListFilter.DangerList.Danger2.DangerItems = dangerList.Where(x => x.Danger2 > 0).Select(x => x.Danger2).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "ABI " + x })
                .OrderBy(x => x.Danger);

            filter.DropdownListFilter.DangerList.Danger3.DangerItems = dangerList.Where(x => x.Danger3 > 0).Select(x => x.Danger3).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Other Sports " + x })
                .OrderBy(x => x.Danger);

            filter.DropdownListFilter.DangerList.Danger4.DangerItems = dangerList.Where(x => x.Danger4 > 0).Select(x => x.Danger4).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "Basketball " + x })
                .OrderBy(x => x.Danger);

            filter.DropdownListFilter.DangerList.Danger5.DangerItems = dangerList.Where(x => x.Danger5 > 0).Select(x => x.Danger5).Distinct()
                .Select(x => new DangerItemViewModel { Danger = x, DangerName = "E-Sports " + x })
                .OrderBy(x => x.Danger);

            if (dangerList.Any(x => x.DangerToString == "-"))
            {
                filter.DropdownListFilter.DangerList.NoneDanger.DangerItems = new List<DangerItemViewModel> { new DangerItemViewModel { Danger = 0, DangerName = "-" } };
            }
            #endregion

            return filter;
        }
    }
}
