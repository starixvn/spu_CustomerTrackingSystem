namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.MatchMonitor;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Implement;
    using System.Threading.Tasks;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Criteria.MatchMonitor.Parlay;
    using Fanex.NAP.CTS.Model.MatchMonitor.Parlay;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.Parlay;

    public class MatchMonitorService : IMatchMonitorService
    {
        private readonly IDynamicRepository _repository;
        private readonly ICommonService _commonService;
        private readonly MMQueryType _mmQueryType;
        private readonly IMemoryCacheService _memoryCacheService;

        public MatchMonitorService(MMQueryType mmQueryType)
        {
            _repository = new DynamicRepository();
            _commonService = new CommonService();
            _mmQueryType = mmQueryType;
            _memoryCacheService = new MemoryCacheService();
        }

        public IEnumerable<MatchFromMainInfoModel> GetMatchFromMainInfos(MatchMonitorSearchViewModel filter, string matchIDs)
        {
            var sportList = filter?.Sports != null ? string.Join(",", filter.Sports.Select(x => x.SportTypeID).Distinct()) : null;

            var criteria = new GetMatchFromMainInfoCriteria
            {
                MatchIDs = matchIDs,
                SportList = sportList,
                FromDate = filter?.FromDate ?? DateTime.Now,
                ToDate = filter?.ToDate ?? DateTime.Now,
                MatchSection = filter.MatchSection,
                Event = filter.Event
            };

            return _repository.Fetch<MatchFromMainInfoModel>(criteria);
        }

        public IEnumerable<GetMatchMonitorModel> GetMatchMonitor(MatchMonitorSearchViewModel filter, string matchInfoFromMainJson)
        {
            var criteria = new MatchMonitorGetCriteria
            {
                FromDate = filter?.FromDate ?? DateTime.Now,
                ToDate = filter?.ToDate ?? DateTime.Now,
                SportType = JsonConvert.SerializeObject(filter.Sports),
                Market = filter.Market,
                BetTypes = JsonConvert.SerializeObject(filter.BetTypes),
                IsVerified = filter.IsVerified,
                Reasons = filter.Reasons,
                MatchInfosJson = matchInfoFromMainJson
            };

            return _repository.Fetch<GetMatchMonitorModel>(criteria);
        }

        public IEnumerable<MMFraudTicketsSummaryViewModel> GetMMFraudTicketsSummary(IEnumerable<MMFraudTicketsSummaryModel> mmModels)
        {
            var criteria = new MMFraudTicketsSummaryCriteria
            {
                FraudTickets = JsonConvert.SerializeObject(mmModels)
            };

            return _repository.Fetch<MMFraudTicketsSummaryViewModel>(criteria);
        }

        public DBResult VerifyTransMatchMonitorDetail(MatchMonitorDetailVerifyTransParaModel para)
        {
            var criteria = new MatchMonitorDetailVerifyCriteria
            {
                MatchID = para.MatchId,
                LiveIndicator = para.LiveIndicator,
                BettypeID = para.BettypeID,
                BetID = para.BetID,
                TranIds = para.TranIds,
                VerifiedBy = para.VerifiedBy,
                TicketType = (int)para.TicketType
            };

            _repository.Fetch<DBResult>(criteria);

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public MMDetailResultModel GetMatchMonitorDetail(MMDetailInfoViewModel filter)
        {
            var criteria = new MatchDetailMonitorGetCriteria
            {
                MatchID = filter.MatchID,
                LiveIndicator = filter.LiveIndicator,
                BettypeID = filter.BetTypeID,
                BetID = filter.BetID,
            };


            (var custs, var matchDetails, var verifiedTrans) =
                _repository.FetchMultiple<MMDetailCustModel, MatchMonitorDetailModel, MatchMonitorDetailVerifyTransResultModel>(criteria);

            return new MMDetailResultModel
            {
                Custs = custs?.ToList() ?? new List<MMDetailCustModel>(),
                MatchDetails = matchDetails?.ToList() ?? new List<MatchMonitorDetailModel>(),
                VerifiedTrans = verifiedTrans?.ToList() ?? new List<MatchMonitorDetailVerifyTransResultModel>()
            };
        }

        private IEnumerable<CustInfoModel> GetCustInfoByCustID(IEnumerable<long> custIds)
        {
            var custIdsToString = string.Join(",", custIds);
            if (!string.IsNullOrWhiteSpace(custIdsToString))
            {
                return GetCustInfoByCustID(custIdsToString);
            }

            return Enumerable.Empty<CustInfoModel>();
        }

        public IEnumerable<CustInfoModel> GetCustInfoByCustID(string custIDs)
        {
            var criteria = new CustInfoByCustIdGetCriteria()
            {
                CustIDs = custIDs
            };

            return _repository.Fetch<CustInfoModel>(criteria);
        }

        public IEnumerable<BetTypeSettingModel> GetBetTypeSettings(BetTypeSettingParaModel infos)
        {
            return _commonService.GetBetTypeSettings(infos);
        }

        public MMDetailFilterViewModel GetFraudFilterInfoDetail(MMDetailInfoViewModel para, TicketDetailViewModel fraudTicket, IEnumerable<StaticListModel> reasons)
        {
            var fraudTicketMergeAGroup = MergeFraudReasonAGroup(fraudTicket, reasons);

            var suspiciousTransInfos = fraudTicket.GetSuspiciousTransInfo(fraudTicketMergeAGroup);

            var mmticketDetailInfo = GetMMTicketDetailInfo(para, suspiciousTransInfos);

            var allFilterResult = fraudTicket.GenerateDropdownFilterListByMainTicketInfo(mmticketDetailInfo.TicketInfoList, para.LiveIndicator, para.Sport);
            allFilterResult.MaxSequenceId = mmticketDetailInfo.MaxSequenceId;
            allFilterResult.MaxTransDate = mmticketDetailInfo.MaxTransDate?.ToString();
            allFilterResult.RecordsTotal = mmticketDetailInfo.TotalTicketAll;

            return allFilterResult;
        }

        public MMDetailFilterViewModel GetAllFilterInfoDetail(MMDetailInfoViewModel para)
        {
            var mmticketDetailInfo = GetMMTicketDetailInfo(para, null);

            var allFilterResult = new TicketDetailViewModel().GenerateDropdownFilterListByMainTicketInfo(mmticketDetailInfo.TicketInfoList, para.LiveIndicator, para.Sport);
            allFilterResult.MaxSequenceId = mmticketDetailInfo.MaxSequenceId;
            allFilterResult.MaxTransDate = mmticketDetailInfo.MaxTransDate?.ToString(FormatDatetimeToString_YYYYmmddHHmmssfff);
            allFilterResult.RecordsTotal = mmticketDetailInfo.TotalTicketAll;

            return allFilterResult;
        }

        public IEnumerable<MatchMonitorDetailItemViewModel> MergeFraudToAllMMDetail(IEnumerable<MatchMonitorDetailItemViewModel> mdItemList, IEnumerable<MatchMonitorDetailItemViewModel> fraudMDItemList)
        {
            if (fraudMDItemList != null && fraudMDItemList.Any())
            {
                var fraudItemDic = fraudMDItemList.ToDictionary(x => x.TransId, x => x);

                return mdItemList.Select(x => x.MappingFraudTicketToAllTicket(fraudItemDic));
            }

            return mdItemList;
        }

        public IEnumerable<MatchMonitorDetailItemViewModel> MergeFraudReasonAGroup(TicketDetailViewModel fraudTicket, IEnumerable<StaticListModel> reasons)
        {
            var mdResult = Enumerable.Empty<MatchMonitorDetailItemViewModel>();

            var matchDetailInfo = fraudTicket.MatchDetailInfo;

            if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
            {
                var mmStatic = new MatchMonitorStaticListModel();
                var ticketDetail = fraudTicket.TicketDetails;
                var mergeGroup = fraudTicket.MMDetailMergeGroup;

                var custIDs = matchDetailInfo.Custs.Select(x => x.CustID).Distinct();
                var custInfo = GetCustInfoByCustID(custIDs);

                mdResult = DrawColorFraudMMDetailWithMergeReasonAGroup(matchDetailInfo.VerifiedTrans, custInfo, ticketDetail, mergeGroup, reasons);
            }

            return mdResult;
        }

        public IEnumerable<MatchMonitorDetailItemViewModel> DrawColorFraudMMDetailWithMergeReasonAGroup(IEnumerable<MatchMonitorDetailVerifyTransResultModel> verifiedTrans,
            IEnumerable<CustInfoModel> custInfo,
            IEnumerable<TicketDetailModel> ticket,
            MMDetailMergeGroupViewModel mergeGroup,
            IEnumerable<StaticListModel> reasons)
        {
            var ticketResult = Enumerable.Empty<MatchMonitorDetailItemViewModel>();

            if (custInfo == null || ticket == null || mergeGroup.TicketMergeGroups == null)
                return ticketResult;

            var isViewParlay = mergeGroup.TicketType == Model.TicketType.Parlay;

            if (isViewParlay)
                ticket = ticket.Where(x => x.TicketOrder == 0);

            var dicCustInfo = custInfo.ToDictionary(x => x.CustID, x => x);
            var dicVerifiedTrans = verifiedTrans.Distinct(x => x.VerifiedTransID).ToDictionary(x => x.VerifiedTransID, x => x);
            var dicGroup = mergeGroup.Groups.ToDictionary(x => x.GroupID, x => x);

            ticketResult = mergeGroup.TicketMergeGroups.Join(ticket,
                    tg => isViewParlay ? tg.Refno : tg.TransID,
                    t => isViewParlay ? t.Refno : t.TransId,
                (tg, t) => new { tg = tg, t = t }).Select(x =>
                {
                    dicCustInfo.TryGetValue(x.t.CustId, out CustInfoModel xCustInfo);
                    dicVerifiedTrans.TryGetValue(x.t.TransId, out MatchMonitorDetailVerifyTransResultModel xVerifiedTran);
                    dicGroup.TryGetValue(x.tg.MergeGroupID, out GroupInfoViewModel group);

                    return new MatchMonitorDetailItemViewModel(x.tg.MergeGroupID, x.t, xCustInfo ?? new CustInfoModel())
                    {
                        PICVerified = xVerifiedTran?.VerifiedTransID.ToString() ?? string.Empty,
                        ReasonList = GetReasonName(reasons, x.tg.ReasonList),
                        ReasonIDList = x.tg.ReasonList,
                        ADetectGroupID = group?.GroupID ?? 0,
                        ADetectGroup = group?.GroupName ?? "-"
                    };
                });

            return ticketResult.Distinct(x => isViewParlay ? x.Refno : x.TransId);
        }

        public string GetReasonName(IEnumerable<StaticListModel> reasonList, string reasonIDList)
        {
            if (!string.IsNullOrEmpty(reasonIDList))
            {
                var arrReasons = reasonIDList.Split(',');
                var reasonNames = reasonList.Join(arrReasons,
                    r1 => r1.ItemValue,
                    r2 => r2,
                    (r1, r2) => r1.ItemNameDisplay);

                return string.Join(",", reasonNames);
            }

            return string.Empty;
        }

        public string GetTransFilterSuspicousMGr(MMDetailFilterViewModel filter, TicketDetailViewModel fraudTicket, TicketType ticketType = TicketType.Single)
        {
            var transJson = string.Empty;

            var isGetAll = (filter.ReasonList != null && filter.ReasonList.Any(x => x.Equals("-"))
               && (filter.MGroupList != null && filter.MGroupList.Any(x => x.Equals("-") || x.Equals("0")))
               && (filter.AGroupList != null && filter.AGroupList.Any(x => x.Equals("-"))));

            if (!isGetAll || filter.IsFilterSuspicious)
            {
                var isExclude = (filter.ReasonList != null && filter.ReasonList.Any(x => x.Equals("-"))) &&
                        (filter.AGroupList != null && filter.AGroupList.Any(x => x.Equals("-")));

                if (fraudTicket != null)
                {
                    if (isExclude)
                    {
                        transJson = fraudTicket.GetTransByFilterExcludeSuspicous(filter, fraudTicket.AGroupReasonMerged);
                    }
                    else
                    {
                        transJson = fraudTicket.GetTransByFilterIncludeSuspicous(filter, fraudTicket.AGroupReasonMerged);
                    }
                }
                else
                {
                    fraudTicket = new TicketDetailViewModel();
                    fraudTicket.TicketType = ticketType;

                    if (isExclude)
                    {
                        transJson = fraudTicket.GetTransByFilterExcludeMGr(filter);
                    }
                    else
                    {
                        transJson = fraudTicket.GetTransByFilterIncludeMGr(filter);
                    }
                }
            }

            return transJson;
        }

        public TicketDetailViewModel BuildSingleTicketDetailFraudMergeGroup(MMDetailResultModel matchDetailInfo, MMDetailInfoViewModel para, TicketDetailViewModel singleTicketDetails)
        {
            if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
            {
                var mergeGroup = new MMDetailMergeGroupViewModel()
                {
                    TicketType = para.TicketType
                };

                var ticketDetails = singleTicketDetails.TicketDetails.Distinct(x => x.TransId);

                var mMDetailBetTeams = matchDetailInfo.MatchDetails.Join(ticketDetails,
                   m => m.TransID,
                   t => t.TransId, (m, t) => new MMDetailBetTeamModel(m, t, false));

                if (mMDetailBetTeams.Any())
                {
                    mergeGroup.TicketMergeGroups = GetMMDetailMergeGroup(mMDetailBetTeams);
                }

                mergeGroup.Groups = mergeGroup.GroupByGroupID();

                singleTicketDetails.MMDetailMergeGroup = mergeGroup;
                singleTicketDetails.MatchDetailInfo = matchDetailInfo;

                var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);
                singleTicketDetails.AGroupReasonMerged = MergeFraudReasonAGroup(singleTicketDetails, reasons);
            }

            return singleTicketDetails;
        }

        public TicketDetailViewModel GetTicketDetailFraud(MMDetailResultModel matchDetailInfo, MMDetailInfoViewModel para)
        {
            var ticketMD = matchDetailInfo.MatchDetails.Select(x =>
                new FraudTransModel
                {
                    TransID = x.TransID,
                    Refno = x.Refno,
                    Reason = x.Reason.ToString(),
                    AGroup = x.GroupID
                });

            var transJsons = JsonConvert.SerializeObject(ticketMD);

            var result = GetTicketDetailByFraudTranIDs(para, null, transJsons);

            var mergeGroup = new MMDetailMergeGroupViewModel()
            {
                TicketType = para.TicketType
            };

            var ticketDetails = Enumerable.Empty<TicketDetailModel>();
            var isViewParlay = para.TicketType == TicketType.Parlay;
            if (para.TicketType == TicketType.Parlay)
            {
                ticketDetails = result.TicketDetails.Where(x => x.TicketOrder == 0).Distinct(x => x.Refno);
            }
            else
            {
                ticketDetails = result.TicketDetails.Distinct(x => x.TransId);
            }

            var mMDetailBetTeams = matchDetailInfo.MatchDetails.Join(ticketDetails,
               m => isViewParlay ? m.Refno : m.TransID,
               t => isViewParlay ? t.Refno : t.TransId, (m, t) => new MMDetailBetTeamModel(m, t, isViewParlay));

            if (mMDetailBetTeams.Any())
            {
                mergeGroup.TicketMergeGroups = GetMMDetailMergeGroup(mMDetailBetTeams);
            }

            mergeGroup.Groups = mergeGroup.GroupByGroupID();

            result.MMDetailMergeGroup = mergeGroup;

            return result;
        }

        public TicketDetailViewModel GetTicketDetailByFraudTranIDs(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, string transJson)
        {
            return GetMainTicketDetail(para, filter, null, transJson);
        }

        public MMDetailResultModel GetMatchMonitorDetailForCache(MMDetailInfoViewModel para, string region, string userId, string viewModelName)
        {
            var key = para.BuildCacheKey("GetMatchMonitorDetailForCache", region, userId, viewModelName);

            var matchDetailInfo = _memoryCacheService.GetValue<MMDetailResultModel>(key);

            if (matchDetailInfo == null)
            {
                matchDetailInfo = GetMatchMonitorDetail(para);

                _memoryCacheService.PushValue(key, matchDetailInfo);
            }

            return matchDetailInfo;
        }

        public TicketDetailViewModel GetSingleTicketDetailForCache(MMDetailResultModel matchDetailInfo, MMDetailInfoViewModel para, string region, string userId, string viewModelName)
        {
            var key = para.BuildCacheKey("GetSingleTicketDetailForCache", region, userId, viewModelName);

            var result = _memoryCacheService.GetValue<TicketDetailViewModel>(key);

            if (result == null)
            {
                var ticketMD = matchDetailInfo?.MatchDetails.Select(x =>
                    new FraudTransModel
                    {
                        TransID = x.TransID,
                        Refno = x.Refno,
                        Reason = x.Reason.ToString(),
                        AGroup = x.GroupID
                    });

                var transJsons = ticketMD != null ? JsonConvert.SerializeObject(ticketMD) : "[]";

                result = GetTicketDetailByFraudTranIDs(para, null, transJsons);

                _memoryCacheService.PushValue(key, result);
            }

            return result;
        }

        public TicketDetailViewModel GetTicketDetailAll(MMDetailInfoViewModel info, MMDetailFilterViewModel filter, string transJson)
        {
            var batchSize = 5000;
            var ticketDetails = new List<TicketDetailModel>();
            var result = new TicketDetailViewModel();
            var isContinue = true;

            if (filter.CustAmountRM > 0)
            {
                result = GetMainTicketDetail(info, filter, -1, transJson);
                info.LastScannedSequenceID = result.MinSequenceId;
            }
            else
            {
                do
                {
                    result = GetMainTicketDetail(info, filter, batchSize, transJson);

                    result.TicketDetails = result.TicketDetails.OrderByDescending(x => x.SequenceID);

                    info.LastScannedSequenceID = result.MinSequenceId;

                    if (result.MinSequenceId != null && result.TicketDetails.Count() > 0)
                    {
                        ticketDetails.AddRange(result.TicketDetails);
                    }

                    if (result.MinSequenceId == null || ticketDetails.Count() >= batchSize)
                    {
                        result.TicketDetails = ticketDetails.ToList();

                        isContinue = false;
                    }

                } while (isContinue);
            }

            return result;
        }

        public TicketDetailViewModel GetMainTicketDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, int? batchSize, string transJson = null)
        {
            switch (para.TicketType)
            {
                case TicketType.Single:
                    {
                        return GetSingleTicketDetail(para, filter, batchSize, transJson);
                    }
                case TicketType.Parlay:
                    {
                        return GetParlayTicketDetail(para, filter, batchSize, transJson);
                    }
            }

            return new TicketDetailViewModel();
        }

        public int GetTotalTicketDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, int? batchSize, string transIDsJson)
        {
            var ticketResult = GetMainTicketDetail(para, filter, batchSize, transIDsJson);

            return ticketResult.TotalTicket ?? 0;
        }

        private string GenerateDangerJson(MMDetailFilterViewModel filter)
        {
            var jsonDanger = new List<MMDetailDangerModel>();

            if (filter.IsDangerNone)
                jsonDanger.Add(new MMDetailDangerModel { Danger1 = 0, Danger2 = 0, Danger3 = 0, Danger4 = 0, Danger5 = 0 });
            filter.Danger1List?.ForEach(x => { jsonDanger.Add(new MMDetailDangerModel { Danger1 = x }); });
            filter.Danger2List?.ForEach(x => { jsonDanger.Add(new MMDetailDangerModel { Danger2 = x }); });
            filter.Danger3List?.ForEach(x => { jsonDanger.Add(new MMDetailDangerModel { Danger3 = x }); });
            filter.Danger4List?.ForEach(x => { jsonDanger.Add(new MMDetailDangerModel { Danger4 = x }); });
            filter.Danger5List?.ForEach(x => { jsonDanger.Add(new MMDetailDangerModel { Danger5 = x }); });

            return JsonConvert.SerializeObject(jsonDanger);
        }

        public MMTicketDetailInfoModel GetMMTicketDetailInfo(MMDetailInfoViewModel para, IEnumerable<SuspiciousTransInfoModel> suspiciousTranInfos)
        {
            if (para.TicketType == Model.TicketType.Parlay)
            {
                return GetMMParlayTicketInfo(para, suspiciousTranInfos);
            }

            var criteria = new MMTicketDetailInfoCriteria()
            {
                MatchID = para.MatchID,
                EventDate = para.EventDate,
                IsLive = para.LiveIndicator ? 1 : 0,
                ListSuspiciousTransId = suspiciousTranInfos != null ? JsonConvert.SerializeObject(suspiciousTranInfos) : null,
                ViewMode = (int)GetViewModeFromDB(_mmQueryType, para.TypeOfTickets),
                ListSportBettype = CreateBetTypeInfoJson(para, true),
                IsExclTestCurr = _mmQueryType == MMQueryType.DangerMMReport,
                UserNameList = para.UserNameList,

                FromTransDate = para.TranstimeFrom != null ? para.TranstimeFrom.Value.ToUniversalTime() : (DateTime?)null,
                ToTransDate = para.TranstimeTo != null ? para.TranstimeTo.Value.ToUniversalTime() : (DateTime?)null
            };

            var result = _repository.Fetch<MMDetailFilterTicketInfoModel>(criteria);

            return new MMTicketDetailInfoModel
            {
                MaxSequenceId = criteria.MaxSequenceId,
                MaxTransDate = criteria.MaxTransDate,
                TotalTicketAll = criteria.TotalTicketAll,
                TotalTicketCredit = criteria.TotalTicketCredit,
                TotalTicketLicensee = criteria.TotalTicketLicensee,
                TicketInfoList = result
            };
        }

        public MMTicketDetailInfoModel GetMMParlayTicketInfo(MMDetailInfoViewModel para, IEnumerable<SuspiciousTransInfoModel> suspiciousRefnos)
        {
            var betTypeInfos = JsonConvert.SerializeObject(new List<MatchBetTypeSettingModel>() { new MatchBetTypeSettingModel {
                SportTypeID = para.Sport,
                BetTypeID = para.BetTypeID,
                BetChoiceType = para.BetChoiceType,
                BetChoiceHome = para.BetChoiceHome,
                BetChoiceAway = para.BetChoiceAway,
                BetID = long.Parse(para.BetID)
            } });

            var criteria = new ParlayTicketInfoGetCriteria()
            {
                MatchID = para.MatchID,
                EventDate = para.EventDate,
                IsLive = para.LiveIndicator ? 1 : 0,
                ListSuspiciousRefno = suspiciousRefnos != null ? JsonConvert.SerializeObject(suspiciousRefnos) : null,
                //ListSuspiciousTransIdm=  null,
                ViewMode = (int)GetViewModeFromDB(_mmQueryType, para.TypeOfTickets),
                ListSportBettype = betTypeInfos,
                IsExclTestCurr = _mmQueryType == MMQueryType.DangerMMReport,
                FromTransDate = para.TranstimeFrom != null ? para.TranstimeFrom.Value.ToUniversalTime() : (DateTime?)null,
                ToTransDate = para.TranstimeTo != null ? para.TranstimeTo.Value.ToUniversalTime() : (DateTime?)null
            };

            var result = _repository.Fetch<MMDetailFilterTicketInfoModel>(criteria);

            return new MMTicketDetailInfoModel
            {
                MaxSequenceId = criteria.MaxSequenceId,
                MaxTransDate = criteria.MaxTransDate,
                TotalTicketAll = criteria.TotalTicketAll,
                TotalTicketCredit = criteria.TotalTicketCredit,
                TotalTicketLicensee = criteria.TotalTicketLicensee,
                TicketInfoList = result
            };
        }

        public IEnumerable<MMDetailMergeGroupModel> GetMMDetailMergeGroup(IEnumerable<MMDetailBetTeamModel> mmDetailBetTeams)
        {
            var transList = JsonConvert.SerializeObject(mmDetailBetTeams);

            var criteria = new MMDetailMergeGroupCriteria
            {
                TransList = transList
            };

            return _repository.Fetch<MMDetailMergeGroupModel>(criteria);
        }

        public IEnumerable<RobotStatusModel> GetRobotStatus(string transJson)
        {
            var criteria = new RobotStatusByTransGetCriteria()
            {
                TransJson = transJson
            };

            return _repository.Fetch<RobotStatusModel>(criteria);
        }

        #region Export Excel

        public byte[] ExportExcelMMDetailSG(IEnumerable<ExExcelTicketInfoViewModel> items, string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            FillRowDataSG(items, ref writer);

            writer.SyncColumnsWidth(17);

            return writer.ToBytes();
        }

        private void FillRowDataSG(IEnumerable<ExExcelTicketInfoViewModel> items, ref ExcelTemplateWriter writer)
        {
            var arrItems = items != null ? items.ToArray() : new ExExcelTicketInfoViewModel[0];
            var lengthItems = arrItems.Length;

            for (int i = 0; i < lengthItems; i++)
            {
                var item = arrItems[i];
                var obj = new object[] {
                    item.SubscriberType,
                    item.Username,
                    item.CustomerClass == -1 ? "-" : item.CustomerClass.ToString(),
                    (item.IsRobot ? "x" : "-"),
                    item.DangerToString,
                    item.Odds,
                    item.MalayOdds,
                    item.BetID.ToString(),
                    item.Currency,
                    (item.Amount % 1 == 0 ? item.Amount.ToString(Consts.FormatDecimalToString_Thousands) : item.Amount.ToString()),
                    item.BetTeam,
                    item.Score,
                    item.TransTimeToString,
                    item.BetFrom,
                    item.ADetectGroup,
                    item.MDetectGroup,
                    item.ReasonList,
                    (!string.IsNullOrEmpty(item.PICVerified) ? "x" : "-")
                };

                writer.WriteRow(1, obj);
            }
        }

        public byte[] ExportExcelMMDetailDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems
            , string fileName, string templatePath, bool isViewParlay, int sportType)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            switch (isViewParlay)
            {
                case true:
                    FillRowDataParlayDG(homeMItems, awayMItems, ref writer);
                    writer.SyncColumnsWidth(37);
                    break;
                case false when(sportType == (int)MatchMonitorSportType.Badminton):
                    FillRowDataBadmintonDG(homeMItems, awayMItems, ref writer);
                    writer.SyncColumnsWidth(39);
                    break;
                case false when (sportType != (int)MatchMonitorSportType.Badminton):
                    FillRowDataDG(homeMItems, awayMItems, ref writer);
                    writer.SyncColumnsWidth(37);
                    break;
                default:
                    break;
            }

            return writer.ToBytes();
        }

        private void FillRowDataDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems,
            IEnumerable<ExExcelTicketInfoViewModel> awayMItems,
            ref ExcelTemplateWriter writer)
        {
            var tblHome = homeMItems != null ? homeMItems.ToArray() : new ExExcelTicketInfoViewModel[0];
            var tblAway = awayMItems != null ? awayMItems.ToArray() : new ExExcelTicketInfoViewModel[0];
            var lengthH = tblHome.Length;
            var lengthA = tblAway.Length;

            if (lengthH > lengthA)
            {
                for (int i = 0; i < lengthH; i++)
                {
                    if (i < lengthA)
                        writer.WriteRow(1, NewRecordTblHomeAwayDG(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblHomeDG(tblHome[i]));
                }
            }
            else
            {
                for (int i = 0; i < lengthA; i++)
                {
                    if (i < lengthH)
                        writer.WriteRow(1, NewRecordTblHomeAwayDG(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblAwayDG(tblAway[i]));
                }
            }
        }

        private void FillRowDataBadmintonDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems,
            IEnumerable<ExExcelTicketInfoViewModel> awayMItems,   
            ref ExcelTemplateWriter writer)
        {
            var tblHome = homeMItems != null ? homeMItems.ToArray() : new ExExcelTicketInfoViewModel[0];
            var tblAway = awayMItems != null ? awayMItems.ToArray() : new ExExcelTicketInfoViewModel[0];
            var lengthH = tblHome.Length;
            var lengthA = tblAway.Length;

            if (lengthH > lengthA)
            {
                for (int i = 0; i < lengthH; i++)
                {
                    if (i < lengthA)
                        writer.WriteRow(1, NewRecordTblBadmintonHomeAwayDG(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblBadmintonHomeDG(tblHome[i]));
                }
            }
            else
            {
                for (int i = 0; i < lengthA; i++)
                {
                    if (i < lengthH)
                        writer.WriteRow(1, NewRecordTblBadmintonHomeAwayDG(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblBadmintonAwayDG(tblAway[i]));
                }
            }
        }

        private void FillRowDataParlayDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems,
            IEnumerable<ExExcelTicketInfoViewModel> awayMItems,
            ref ExcelTemplateWriter writer)
        {
            var tblHome = homeMItems != null ? TransformParlayToDetailItems(homeMItems).ToArray() : new ExExcelParlayTicketInfoViewModel[0];
            var tblAway = awayMItems != null ? TransformParlayToDetailItems(awayMItems).ToArray() : new ExExcelParlayTicketInfoViewModel[0];
            var lengthH = tblHome.Length;
            var lengthA = tblAway.Length;

            if (lengthH > lengthA)
            {
                for (int i = 0; i < lengthH; i++)
                {
                    if (i < lengthA)
                        writer.WriteRow(1, NewRecordTblParlayHomeAwayDG(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblParlayHomeDG(tblHome[i]));
                }
            }
            else
            {
                for (int i = 0; i < lengthA; i++)
                {
                    if (i < lengthH)
                        writer.WriteRow(1, NewRecordTblParlayHomeAwayDG(tblHome[i], tblAway[i]));
                    else
                        writer.WriteRow(1, NewRecordTblParlayAwayDG(tblAway[i]));
                }
            }
        }

        private object[] NewRecordTblBadmintonHomeDG(ExExcelTicketInfoViewModel itemH)
        {
            return new object[]
               {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    (itemH.IsRobot ? "x" : "-"),
                    itemH.DangerToString,
                    (itemH.Game == 0 ? "-" : itemH.Game.ToString()), 
                    itemH.Odds,
                    itemH.MalayOdds,
                    itemH.BetID.ToString(),
                    itemH.Currency,
                    (itemH.Amount % 1 == 0 ? itemH.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemH.Amount.ToString()),
                    itemH.HDP,
                    itemH.Score,
                    itemH.TransTimeToString,
                    itemH.BetFrom,
                    itemH.ADetectGroup,
                    itemH.MDetectGroup,
                    itemH.ReasonList,
                    (!string.IsNullOrEmpty(itemH.PICVerified) ? "x" : "-"),
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null
               };
        }

        private object[] NewRecordTblBadmintonAwayDG(ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    (itemA.IsRobot ? "x" : "-"),
                    itemA.DangerToString,
                    (itemA.Game == 0 ? "-" : itemA.Game.ToString()),
                    itemA.Odds,
                    itemA.MalayOdds,
                    itemA.BetID.ToString(),
                    itemA.Currency,
                    (itemA.Amount % 1 == 0 ? itemA.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemA.Amount.ToString()),
                    itemA.HDP,
                    itemA.Score,
                    itemA.TransTimeToString,
                    itemA.BetFrom,
                    itemA.ADetectGroup,
                    itemA.MDetectGroup,
                    itemA.ReasonList,
                    (!string.IsNullOrEmpty(itemA.PICVerified) ? "x" : "-")
                };
        }

        private object[] NewRecordTblBadmintonHomeAwayDG(ExExcelTicketInfoViewModel itemH, ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    (itemH.IsRobot ? "x" : "-"),
                    itemH.DangerToString,
                    (itemH.Game == 0 ? "-" : itemH.Game.ToString()),
                    itemH.Odds,
                    itemH.MalayOdds,
                    itemH.BetID.ToString(),
                    itemH.Currency,
                    (itemH.Amount % 1 == 0 ? itemH.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemH.Amount.ToString()),
                    itemH.HDP,
                    itemH.Score,
                    itemH.TransTimeToString,
                    itemH.BetFrom,
                    itemH.ADetectGroup,
                    itemH.MDetectGroup,
                    itemH.ReasonList,
                    (!string.IsNullOrEmpty(itemH.PICVerified) ? "x" : "-"),
                    "",
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    (itemA.IsRobot ? "x" : "-"),
                    itemA.DangerToString,
                    (itemA.Game == 0 ? "-" : itemA.Game.ToString()),
                    itemA.Odds,
                    itemA.MalayOdds,
                    itemA.BetID.ToString(),
                    itemA.Currency,
                    (itemA.Amount % 1 == 0 ? itemA.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemA.Amount.ToString()),
                    itemA.HDP,
                    itemA.Score,
                    itemA.TransTimeToString,
                    itemA.BetFrom,
                    itemA.ADetectGroup,
                    itemA.MDetectGroup,
                    itemA.ReasonList,
                    (!string.IsNullOrEmpty(itemA.PICVerified) ? "x" : "-")
                };
        }

        private object[] NewRecordTblHomeDG(ExExcelTicketInfoViewModel itemH)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    (itemH.IsRobot ? "x" : "-"),
                    itemH.DangerToString,
                    itemH.Odds,
                    itemH.MalayOdds,
                    itemH.BetID.ToString(),
                    itemH.Currency,
                    (itemH.Amount % 1 == 0 ? itemH.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemH.Amount.ToString()),
                    itemH.HDP,
                    itemH.Score,
                    itemH.TransTimeToString,
                    itemH.BetFrom,
                    itemH.ADetectGroup,
                    itemH.MDetectGroup,
                    itemH.ReasonList,
                    (!string.IsNullOrEmpty(itemH.PICVerified) ? "x" : "-"),
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null,
                    null, null, null, null,
                    null, null, null, null
                };
        }

        private object[] NewRecordTblAwayDG(ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    null, null, null, null,
                    null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    (itemA.IsRobot ? "x" : "-"),
                    itemA.DangerToString,
                    itemA.Odds,
                    itemA.MalayOdds,
                    itemA.BetID.ToString(),
                    itemA.Currency,
                    (itemA.Amount % 1 == 0 ? itemA.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemA.Amount.ToString()),
                    itemA.HDP,
                    itemA.Score,
                    itemA.TransTimeToString,
                    itemA.BetFrom,
                    itemA.ADetectGroup,
                    itemA.MDetectGroup,
                    itemA.ReasonList,
                    (!string.IsNullOrEmpty(itemA.PICVerified) ? "x" : "-")
                };
        }

        private object[] NewRecordTblHomeAwayDG(ExExcelTicketInfoViewModel itemH, ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    (itemH.IsRobot ? "x" : "-"),
                    itemH.DangerToString,
                    itemH.Odds,
                    itemH.MalayOdds,
                    itemH.BetID.ToString(),
                    itemH.Currency,
                    (itemH.Amount % 1 == 0 ? itemH.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemH.Amount.ToString()),
                    itemH.HDP,
                    itemH.Score,
                    itemH.TransTimeToString,
                    itemH.BetFrom,
                    itemH.ADetectGroup,
                    itemH.MDetectGroup,
                    itemH.ReasonList,
                    (!string.IsNullOrEmpty(itemH.PICVerified) ? "x" : "-"),
                    "",
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    (itemA.IsRobot ? "x" : "-"),
                    itemA.DangerToString,
                    itemA.Odds,
                    itemA.MalayOdds,
                    itemA.BetID.ToString(),
                    itemA.Currency,
                    (itemA.Amount % 1 == 0 ? itemA.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemA.Amount.ToString()),
                    itemA.HDP,
                    itemA.Score,
                    itemA.TransTimeToString,
                    itemA.BetFrom,
                    itemA.ADetectGroup,
                    itemA.MDetectGroup,
                    itemA.ReasonList,
                    (!string.IsNullOrEmpty(itemA.PICVerified) ? "x" : "-")
                };
        }

        private object[] NewRecordTblParlayHomeDG(ExExcelParlayTicketInfoViewModel itemH)
        {
            return new object[]
               {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    (itemH.IsRobot ? "x" : "-"),
                    itemH.DangerToString,
                    itemH.Odds,
                    null,
                    itemH.BetID??null,
                    itemH.MatchName,
                    itemH.Currency,
                    (itemH.Amount % 1 == 0 ? itemH.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemH.Amount.ToString()),
                    itemH.Choice,
                    itemH.Score,
                    itemH.TransTimeToString,
                    itemH.BetFrom,
                    itemH.ADetectGroup,
                    itemH.MDetectGroup,
                    itemH.ReasonList,
                    (!string.IsNullOrEmpty(itemH.PICVerified) ? "x" : "-"),
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null
               };
        }

        private object[] NewRecordTblParlayAwayDG(ExExcelParlayTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    (itemA.IsRobot ? "x" : "-"),
                    itemA.DangerToString,
                    itemA.Odds,
                    null,
                    itemA.BetID??null,
                    itemA.MatchName,
                    itemA.Currency,
                    (itemA.Amount % 1 == 0 ? itemA.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemA.Amount.ToString()),
                    itemA.Choice,
                    itemA.Score,
                    itemA.TransTimeToString,
                    itemA.BetFrom,
                    itemA.ADetectGroup,
                    itemA.MDetectGroup,
                    itemA.ReasonList,
                    (!string.IsNullOrEmpty(itemA.PICVerified) ? "x" : "-")
                };
        }

        private object[] NewRecordTblParlayHomeAwayDG(ExExcelParlayTicketInfoViewModel itemH, ExExcelParlayTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    (itemH.IsRobot ? "x" : "-"),
                    itemH.DangerToString,
                    itemH.Odds,
                    (itemH.MalayOdds == "-" ? null : itemH.MalayOdds),
                    itemH.BetID??null,
                    itemH.MatchName,
                    itemH.Currency,
                    (itemH.Amount % 1 == 0 ? itemH.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemH.Amount.ToString()),
                    itemH.Choice,
                    itemH.Score,
                    itemH.TransTimeToString,
                    itemH.BetFrom,
                    itemH.ADetectGroup,
                    itemH.MDetectGroup,
                    itemH.ReasonList,
                    (!string.IsNullOrEmpty(itemH.PICVerified) ? "x" : "-"),
                    "",
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    (itemA.IsRobot ? "x" : "-"),
                    itemA.DangerToString,
                    itemA.Odds,
                    null,
                    itemA.BetID??null,
                    itemA.MatchName,
                    itemA.Currency,
                    (itemA.Amount % 1 == 0 ? itemA.Amount.ToString(Consts.FormatDecimalToString_Thousands) : itemA.Amount.ToString()),
                    itemA.Choice,
                    itemA.Score,
                    itemA.TransTimeToString,
                    itemA.BetFrom,
                    itemA.ADetectGroup,
                    itemA.MDetectGroup,
                    itemA.ReasonList,
                    (!string.IsNullOrEmpty(itemA.PICVerified) ? "x" : "-")
                };
        }

        private IEnumerable<ExExcelParlayTicketInfoViewModel> TransformParlayToDetailItems(IEnumerable<ExExcelTicketInfoViewModel> items)
        {
            var list = new List<ExExcelParlayTicketInfoViewModel>();

            foreach (var single in items)
            {
                list.Add(new ExExcelParlayTicketInfoViewModel(single));

                for (int i = single.ParlayTickets.Count - 1; i >= 0; i--)
                {
                    list.Add(new ExExcelParlayTicketInfoViewModel(single, single.ParlayTickets[i]));
                }
            }

            return list;
        }

        #endregion

        #region User Feedback
        public IEnumerable<GetMMUserFeedbackModel> GetUserFeedback(UserFeedbackViewModel para)
        {
            var criteria = new GetMMUserFeedbackCriteria
            {
                MatchID = para.MatchID,
                BetTypeID = para.BetTypeID,
                LiveIndicator = para.IsLive,
                BetIDs = para.BetID
            };

            return _repository.Fetch<GetMMUserFeedbackModel>(criteria);
        }

        public DBResult InsertUserFeedback(InsertUserFeedbackViewModel para)
        {
            var criteria = new InsertMMUserFeedbackCriteria
            {
                MatchID = para.MatchID,
                SportType = para.SportType,
                LiveIndicator = para.IsLive,
                BettypeID = para.BettypeID,
                BetIDs = para.BetID,
                UserID = para.UserID,
                UserComment = para.UserComment ?? string.Empty,
                IncorrectReason = para.IncorrectReason,
                CorrectReasonID = para.CorrectReasonID
            };

            _repository.Fetch<DBResult>(criteria);

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public DBResult UpdateUserFeedback(UpdateUserFeedbackViewModel para)
        {
            var criteria = new UpdateMMUserFeedbackCriteria
            {
                UserID = para.UserID,
                MMUserCommentID = para.MMUserCommentID,
                UserComment = para.UserComment ?? string.Empty,
                IncorrectReason = para.IncorrectReason,
                CorrectReasonID = para.CorrectReasonID
            };

            _repository.Fetch<DBResult>(criteria);

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public DBResult DeleteUserFeedback(DeleteUserFeedbackViewModel para)
        {
            var criteria = new DeleteMMUserFeedbackCriteria
            {
                UserID = para.UserID,
                MMUserCommentID = para.MMUserCommentID
            };

            _repository.Fetch<DBResult>(criteria);

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        #endregion

        #region Hold Key

        public HoldKeyStatusGetResultModel GetHoldKeyStatus(Int64 matchID, string url)
        {
            if (matchID != 0 && !string.IsNullOrEmpty(url))
            {
                var para = new List<Int64> { matchID };

                var result = ApiHelper.Post<HoldKeyStatusGetResultModel>(url, para, 120);

                Task.WhenAll(result);

                return result.Result;
            }

            return new HoldKeyStatusGetResultModel();
        }

        public HoldKeyUpdateResultModel UpdateHoldKeyStatus(HoldKeyUpdateViewModel parameters, string url)
        {
            if (parameters.MatchId != 0 && parameters.UserId != 0 && !string.IsNullOrEmpty(url))
            {
                var para = new List<HoldKeyUpdateModel>()
                {
                    new HoldKeyUpdateModel
                    {
                        matchId =  parameters.MatchId,
                        sporttype = parameters.SportType,
                        enabled = parameters.IsHold,
                        userId = parameters.UserId,
                        userName = parameters.UserName
                    }
                };

                var result = ApiHelper.Post<HoldKeyUpdateResultModel>(url, para, 120);

                Task.WhenAll(result);

                return result.Result;
            }

            return new HoldKeyUpdateResultModel();
        }

        public void LogHoldKeyAction(int userId, bool status, int matchId)
        {
            if (userId != 0)
            {
                var criteria = new HoldKeyLogInsertCriteria
                {
                    UserID = userId,
                    Status = status,
                    MatchID = matchId
                };

                _repository.Execute(criteria);
            }
        }

        #endregion

        #region Parlay
        public IEnumerable<MMFraudTicketsSummaryViewModel> GetMMParlayFraudTicketsSummary(IEnumerable<MMFraudTicketsSummaryModel> mmModels)
        {
            var criteria = new MMParlayFraudTicketsSummaryCriteria
            {
                FraudTickets = JsonConvert.SerializeObject(mmModels)
            };

            return _repository.Fetch<MMFraudTicketsSummaryViewModel>(criteria);
        }

        public MMDetailResultModel GetMatchMonitorParlayDetail(MMDetailInfoViewModel filter)
        {
            var criteria = new MMParlaySuspiciousTicketGetCriteria
            {
                MatchID = filter.MatchID,
                LiveIndicator = filter.LiveIndicator,
                BettypeID = filter.BetTypeID,
                BetID = long.Parse(filter.BetID),
            };

            (var custs, var matchDetails, var verifiedTrans) =
                _repository.FetchMultiple<MMDetailCustModel, MatchMonitorDetailModel, MatchMonitorDetailVerifyTransResultModel>(criteria);

            return new MMDetailResultModel
            {
                Custs = custs?.ToList() ?? new List<MMDetailCustModel>(),
                MatchDetails = matchDetails?.ToList() ?? new List<MatchMonitorDetailModel>(),
                VerifiedTrans = verifiedTrans?.ToList() ?? new List<MatchMonitorDetailVerifyTransResultModel>()
            };
        }

        #region GetSingleTicketDetail
        private TicketDetailViewModel GetSingleTicketDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, int? batchSize, string transJson = null)
        {
            var criteria = new TicketDetailGetCriteria()
            {
                QueryType = (int)_mmQueryType, // Get ticket for view MM report
                EventDate = para.EventDate,
                LastScannedSequenceID = para.LastScannedSequenceID,
                LastTransDate = para.LastTransDate,
                IsLive = para.LiveIndicator ? 1 : 0,
                MatchID = para.MatchID,
                ListSportBettype = CreateBetTypeInfoJson(para, false),
                ViewMode = para.TypeOfTickets == MMViewModeFromUI.TicketAll ? 0 : 1,
                BatchSize = batchSize,
                IsLicensee = (para.Sites == MMSites.All || para.Sites == null) ? null : (int?)(para.Sites == MMSites.Licensee ? 1 : 0),
                ListTransId = transJson,
                UserNameList = para.UserNameList,
                IsCashout = para.IsCashout,
                FromTransDate = para.TranstimeFrom != null ? para.TranstimeFrom.Value.ToUniversalTime() : (DateTime?)null,
                ToTransDate = para.TranstimeTo != null ? para.TranstimeTo.Value.ToUniversalTime() : (DateTime?)null,
                IsExclTestCurr = _mmQueryType == MMQueryType.DangerMMReport
            };

            if (para.BetChoiceType == 1)
            {
                criteria.ListHDPBetTeam = filter?.BetTeamList == null ? null : string.Join(",", filter.BetTeamList);
            }
            else
            {
                criteria.ListHDPBetTeam = filter?.HDPList == null ? null : string.Join(",", filter.HDPList);
            }

            if (filter?.CustAmountRM > 0)
            {
                criteria.CustAmountRM = filter?.CustAmountRM;
            }

            if (filter != null)
            {
                criteria.ListScore = (filter.ScoreList == null || !para.LiveIndicator || ConditionExcludeScoreForMM.Contains(filter.SportType))
                    ? null : string.Join(",", filter.ScoreList);
                criteria.ListCurrency = filter.CurrencyIdList == null ? null : string.Join(",", filter.CurrencyIdList);
                criteria.ListDanger = GenerateDangerJson(filter);
                criteria.ListStatus = filter.TicketStatusList == null ? null : string.Join(",", filter.TicketStatusList);
                criteria.ListCustomerClass = filter.CustomerClassList == null ? null : string.Join(",", filter.CustomerClassList);
                criteria.ListReason = filter.ReasonList == null ? null : string.Join(",", filter.ReasonList).Replace('-', '0');
                criteria.ListAGroup = filter.AGroupList == null ? null : string.Join(",", filter.AGroupList).Replace('-', '0');
                criteria.ListMGroup = filter.MGroupList == null ? null : string.Join(",", filter.MGroupList).Replace('-', '0');
            }

            var data = _repository.Fetch<TicketDetailModel>(criteria);

            return new TicketDetailViewModel
            {
                TicketDetails = data,
                MinSequenceId = criteria.MinSequenceId,
                MinTransDate = criteria.MinTransDate,
                TotalTicket = criteria.TotalTicket
            };
        }

        private string CreateBetTypeInfoJson(MMDetailInfoViewModel para, bool isSupportGetFilter)
        {
            if (para.Sport == (int)MatchMonitorSportType.Cricket)
            {
                return JsonConvert.SerializeObject(new List<MatchBetTypeSettingModel>()
                {
                    new  MatchBetTypeSettingModel
                    {
                        SportTypeID = para.Sport,
                        BetTypeID = para.BetTypeID,
                        BetChoiceType = para.BetChoiceType,
                        BetChoiceHome = para.BetChoiceHome,
                        BetChoiceAway = para.BetChoiceAway,
                        BetID = null,
                        BetChoiceHomeFullName = isSupportGetFilter ? null : para.BetChoiceHomeFullName,
                        BetChoiceAwayFullName = isSupportGetFilter ? null : para.BetChoiceAwayFullName
                    }});
            }

            var betTypeInfos = para.BetID?.Split(',').Select(x => new MatchBetTypeSettingModel
            {
                SportTypeID = para.Sport,
                BetTypeID = para.BetTypeID,
                BetChoiceType = para.BetChoiceType,
                BetChoiceHome = para.BetChoiceHome,
                BetChoiceAway = para.BetChoiceAway,
                BetID = long.Parse(x),
                BetChoiceHomeFullName = isSupportGetFilter ? null : para.BetChoiceHomeFullName,
                BetChoiceAwayFullName = isSupportGetFilter ? null : para.BetChoiceAwayFullName
            }).ToList();

            return betTypeInfos != null ? JsonConvert.SerializeObject(betTypeInfos) : "[]";
        }
        #endregion

        private TicketDetailViewModel GetParlayTicketDetail(MMDetailInfoViewModel para,
            MMDetailFilterViewModel filter,
            int? batchSize,
            string listRefno = null)
        {
            var criteria = new ParlayTicketDetailGetCriteria()
            {
                EventDate = para.EventDate,
                LastScannedSequenceID = para.LastScannedSequenceID,
                LastTransDate = para.LastTransDate,
                IsLive = para.LiveIndicator ? 1 : 0,
                MatchID = para.MatchID,
                BetType = para.BetTypeID,
                BetID = long.Parse(para.BetID),
                ListBetType = JsonConvert.SerializeObject(
                                _commonService
                                    .GetBetTypeSettings(new BetTypeSettingParaModel() { Status = 1 })
                                    .Select(x => new MatchBetTypeSettingModel(x))),
                ViewMode = para.TypeOfTickets == MMViewModeFromUI.TicketAll ? 0 : 1,
                BatchSize = batchSize,
                IsLicensee = (para.Sites == MMSites.All || para.Sites == null) ? null : (int?)(para.Sites == MMSites.Licensee ? 1 : 0),
                ListRefno = listRefno ?? "[]",
                FromTransDate = para.TranstimeFrom != null ? para.TranstimeFrom.Value.ToUniversalTime() : (DateTime?)null,
                ToTransDate = para.TranstimeTo != null ? para.TranstimeTo.Value.ToUniversalTime() : (DateTime?)null,
                IsExclTestCurr = _mmQueryType == MMQueryType.DangerMMReport
            };

            if (para.BetChoiceType == 1)
            {
                criteria.ListHDPBetTeam = filter?.BetTeamList == null ? null : string.Join(",", filter.BetTeamList);
            }
            else
            {
                criteria.ListHDPBetTeam = filter?.HDPList == null ? null : string.Join(",", filter.HDPList);
            }

            if (filter?.CustAmountRM > 0)
            {
                criteria.CustAmountRM = filter?.CustAmountRM;
            }

            if (filter != null)
            {
                criteria.ListScore = (filter.ScoreList == null || !para.LiveIndicator || filter.SportType == 2) ? null : string.Join(",", filter.ScoreList);
                criteria.ListCurrency = filter.CurrencyIdList == null ? null : string.Join(",", filter.CurrencyIdList);
                criteria.ListDanger = GenerateDangerJson(filter);
                criteria.ListStatus = filter.TicketStatusList == null ? null : string.Join(",", filter.TicketStatusList);
                criteria.ListCustomerClass = filter.CustomerClassList == null ? null : string.Join(",", filter.CustomerClassList);
                criteria.ListReason = filter.ReasonList == null ? null : string.Join(",", filter.ReasonList).Replace('-', '0');
                criteria.ListAGroup = filter.AGroupList == null ? null : string.Join(",", filter.AGroupList).Replace('-', '0');
                criteria.ListMGroup = filter.MGroupList == null ? null : string.Join(",", filter.MGroupList).Replace('-', '0');
            }

            var data = _repository.Fetch<TicketDetailModel>(criteria);

            return new TicketDetailViewModel
            {
                TicketDetails = GroupParlayTickets(data),
                MinSequenceId = criteria.MinSequenceId,
                MinTransDate = criteria.MinTransDate,
                TotalTicket = criteria.TotalTicket
            };
        }

        private IEnumerable<TicketDetailModel> GroupParlayTickets(IEnumerable<TicketDetailModel> tickets)
        {
            var parlaySubDictionary = tickets.Where(x => x.TicketOrder != 0)
                .GroupBy(x => x.Refno, (key, g) => new { Refno = key, ParlayTickets = g.Select(y => new ParlayTicketInfoModel(y)) })
                .ToDictionary(x => x.Refno, x => x.ParlayTickets);

            var groupTickets = tickets.Where(x => x.TicketOrder == 0).Select(x =>
                {
                    parlaySubDictionary.TryGetValue(x.Refno, out var parlayTickets);

                    x.ParlayTickets = parlayTickets.OrderBy(y => y.IsMainTicket);

                    return x;
                });

            return groupTickets;
        }

        #endregion

    }
}
