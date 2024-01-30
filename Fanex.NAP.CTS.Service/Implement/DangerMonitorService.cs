namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Criteria.DangerMonitor;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Fanex.NAP.TGRS.Core.ExcelUtility;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class DangerMonitorService : IDangerMonitorService
    {
        private readonly ICommonService _commonService;
        private readonly IDynamicRepository _repository;
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly IMatchMonitorService _matchMonitorService;

        private readonly string UserID = "MMDetailUserID";
        private readonly string Region = "MMDetailRegion";

        private const int mainBatchSize = 500;
        private const int detailBatchSize = 10;

        public DangerMonitorService(string region, string userID)
        {
            _commonService = new CommonService();
            _repository = new DynamicRepository();
            _matchMonitorService = new MatchMonitorService(MMQueryType.DangerMMReport);

            _memoryCacheService = new MemoryCacheService();

            Region = region;
            UserID = userID;
        }

        #region public method
        public IEnumerable<DangerMonitorBetTypeViewModel> GetSportBettypesFromCache(bool isGetNew = false)
        {
            var result = default(IEnumerable<DangerMonitorBetTypeViewModel>);

            var cacheKey = $"{nameof(GetSportBettypesFromCache)}_{Region}";

            lock (cacheKey)
            {
                result = isGetNew ? null : _memoryCacheService.GetValue<IEnumerable<DangerMonitorBetTypeViewModel>>(cacheKey);

                if (result == null)
                {
                    result = GetSportBettypes();

                    _memoryCacheService.PushValue(cacheKey, result);
                }
            }

            return result;
        }

        public GetDangerMonitorResultViewModel GetDangerMonitor(DangerMonitorSearchModel filter)
        {
            var result = Enumerable.Empty<GetDangerMonitorItemViewModel>();

            if (!string.IsNullOrWhiteSpace(filter.LastMatchSection) && !string.IsNullOrWhiteSpace(filter.MatchSection))
            {
                var dangerMonitor = GetDangerMonitorSummary(filter, mainBatchSize);

                int.TryParse(filter.LastMatchSection, out var matchSection);

                result = dangerMonitor != null ? MappingDangerMonitorWithSportBettype(dangerMonitor.Items, matchSection) : null;

                var isNextMatchSection = string.IsNullOrEmpty(dangerMonitor?.LastEventDate);

                filter.LastMatchSection = isNextMatchSection ? filter.MatchSection.Split(',').Where(x => int.Parse(x) > int.Parse(filter.LastMatchSection)).FirstOrDefault() : filter.LastMatchSection;

                filter.LastMatchID = isNextMatchSection ? null : dangerMonitor?.LastMatchId;

                filter.LastEventDate = isNextMatchSection ? null : dangerMonitor?.LastEventDate;
            }

            return new GetDangerMonitorResultViewModel
            {
                Items = result ?? new List<GetDangerMonitorItemViewModel>(),
                LastMatchID = filter.LastMatchID,
                LastMatchSection = filter.LastMatchSection,
                LastEventDate = filter.LastEventDate
            };
        }

        public byte[] ExportExcelMMDetailSG(IEnumerable<ExExcelTicketInfoViewModel> items, string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            FillRowDataSG(items, ref writer);

            writer.SyncColumnsWidth(14);

            return writer.ToBytes();
        }

        public byte[] ExportExcelMMDetailDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems
            , string fileName, string templatePath)
        {
            var writer = new ExcelTemplateWriter(templatePath, fileName, "Sheet1");
            writer.CopyRows(0, 1);

            FillRowDataDG(homeMItems, awayMItems, ref writer);

            writer.SyncColumnsWidth(29);

            return writer.ToBytes();
        }
        #endregion

        #region private method
        private IEnumerable<GetDangerMonitorItemViewModel> MappingDangerMonitorWithSportBettype(IEnumerable<GetDangerSummaryItemViewModel> dangerMonitors, int matchSection)
        {
            var sportBettypes = GetSportBettypesFromCache();

            return dangerMonitors.Join(sportBettypes,
                                        item => $"{item.SportType}_{item.Bettype}_{item.BetID}",
                                        sportBettype => $"{sportBettype.SportType}_{sportBettype.BetTypeID}_{sportBettype.BetID}",
                                        (item, sportBettype) => new GetDangerMonitorItemViewModel(item, matchSection, sportBettype));
        }

        private IEnumerable<BetTypeSettingModel> GetBetTypeSettings(string sportIDs, bool isFilterMMDetection)
        {
            var paras = new BetTypeSettingParaModel
            {
                FunctionID = (int)BetTypeSettingsFunctionID.DgrBetType,
                BettypeIDList = null,
                SportTypeIDList = sportIDs,
                IsFilterMMDetection = isFilterMMDetection,
                IsShowDetailsX = true,
                Status = 1
            };

            return _commonService.GetBetTypeSettings(paras);
        }

        private GetDangerSummaryResultViewModel GetDangerMonitorSummary(DangerMonitorSearchModel filter, int batchSize = 500)
        {
            var criteria = new DangerMonitorTicketSummaryCriteria
            {
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                SportList = filter.Sports,
                LeagueEvent = filter.Event ?? string.Empty,
                BettypeList = filter.BetTypes,
                DangerList = filter.DangerList,
                MarketList = filter.Market,
                MatchSection = filter.LastMatchSection,
                LastEventDate = filter.LastEventDate,
                LastMatchID = filter.LastMatchID,
                BatchSize = batchSize,
            };

            var response = _repository.FetchMultiple<GetDangerSummaryResultViewModel, GetDangerSummaryItemViewModel>(criteria);

            var result = response.firstResultSet.FirstOrDefault();

            if (result != null)
            {
                result.Items = response.secondResultSet;
            }
            return result;
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
                    item.DangerToString,
                    item.Odds,
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
                };

                writer.WriteRow(1, obj);
            }
        }

        private void FillRowDataDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems, ref ExcelTemplateWriter writer)
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

        private object[] NewRecordTblHomeDG(ExExcelTicketInfoViewModel itemH)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    itemH.DangerToString,
                    itemH.Odds,
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
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                };
        }

        private object[] NewRecordTblAwayDG(ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    null, null, null, null,
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    itemA.DangerToString,
                    itemA.Odds,
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
                };
        }

        private object[] NewRecordTblHomeAwayDG(ExExcelTicketInfoViewModel itemH, ExExcelTicketInfoViewModel itemA)
        {
            return new object[]
                {
                    itemH.SubscriberType,
                    itemH.Username,
                    itemH.CustomerClass == -1 ? "-" : itemH.CustomerClass.ToString(),
                    itemH.DangerToString,
                    itemH.Odds,
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
                    "",
                    itemA.SubscriberType,
                    itemA.Username,
                    itemA.CustomerClass == -1 ? "-" : itemA.CustomerClass.ToString(),
                    itemA.DangerToString,
                    itemA.Odds,
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
                };
        }
        #endregion

        #region mmdetail
        public MMDetailTicketListViewModel GetMatchMonitorDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter)
        {
            var mdResult = default(MMDetailTicketListViewModel);

            switch (para.TypeOfTickets)
            {
                case MMTicketTypeFromUI.TicketFraud:
                    {
                        var fraudTicket = GetFraudMMDetailForCache(para);

                        if (fraudTicket != null)
                        {
                            mdResult = GetFraudMMDetail(para, filter, fraudTicket);
                        }

                        break;
                    }
                case MMTicketTypeFromUI.TicketAll when para.IsSupportShowAllTickets:
                    {
                        mdResult = GetAllMMDetail(para, filter);

                        if (mdResult != null)
                        {
                            var ctsCustIDHomeIsZero = mdResult.HomeMatchItemList.Where(x => x.CTSCustID == 0).Select(x => x);
                            var ctsCustIDAwayIsZero = mdResult.AwayMatchItemList.Where(x => x.CTSCustID == 0).Select(x => x);

                            if (ctsCustIDHomeIsZero != null && ctsCustIDHomeIsZero.Any())
                            {
                                LogService.Error($"{Region} Get Match Monitor Detail with CTSCustIDHome=0: {JsonConvert.SerializeObject(ctsCustIDHomeIsZero)}");
                            }

                            if (ctsCustIDAwayIsZero != null && ctsCustIDAwayIsZero.Any())
                            {
                                LogService.Error($"{Region} Get Match Monitor Detail with CTSCustIDAway=0: {JsonConvert.SerializeObject(ctsCustIDAwayIsZero)}");
                            }
                        }

                        break;
                    }
            }

            return mdResult;
        }

        public MMDetailFilterViewModel GetMatchMonitorFilter(MMDetailInfoViewModel para)
        {
            var result = new MMDetailFilterViewModel();

            if (para.IsImportNewTicket)
            {
                result = GetMatchMonitorFilterImportNew(para);
            }
            else
            {
                if (para.TypeOfTickets == MMTicketTypeFromUI.TicketFraud)
                {
                    result = GetFraudMMFilter(para);
                }
                else if (para.IsSupportShowAllTickets)
                {
                    result = GetAllMMFilter(para);
                }
            }

            return result;
        }

        public long GetMatchMonitorTotalTicket(MMDetailInfoViewModel info, MMDetailFilterViewModel filter)
        {
            long recordsTotal = 0;

            var fraudTicket = GetFraudMMDetailForCache(info);

            var transJson = _matchMonitorService.GetTransFilterSuspicousMGr(filter, fraudTicket);

            recordsTotal = _matchMonitorService.GetTotalTicketDetail(info, filter, 0, transJson);

            return recordsTotal;
        }
        #endregion

        #region private method
        private TicketDetailViewModel GetFraudMMDetailForCache(MMDetailInfoViewModel filter)
        {
            var para = filter.Clone();
            var fraudMMDetailResult = new TicketDetailViewModel();
            var ticketTypeName = Enum.GetName(typeof(MMTicketTypeFromUI), MMTicketTypeFromUI.TicketFraud);
            var key = para.BuildCacheKey("TicketFraud", Region, UserID, ticketTypeName);

            try
            {
                fraudMMDetailResult = _memoryCacheService.GetValue<TicketDetailViewModel>(key);

                if (fraudMMDetailResult == null)
                {
                    para.TypeOfTickets = MMTicketTypeFromUI.TicketFraud;

                    var matchDetailInfo = _matchMonitorService.GetMatchMonitorDetail(para);

                    if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
                    {
                        var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);

                        fraudMMDetailResult = _matchMonitorService.GetTicketDetailFraud(matchDetailInfo, para);

                        fraudMMDetailResult.MatchDetailInfo = matchDetailInfo;

                        fraudMMDetailResult.AGroupReasonMerged = _matchMonitorService.MergeFraudReasonAGroup(fraudMMDetailResult, reasons);

                        _memoryCacheService.PushValue(key, fraudMMDetailResult);
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddMessage($"{Region} Get Fraud MM Detail For Cache Error: " + ex.Message + ex.StackTrace + "\n"
                   + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                   + ex.InnerException.StackTrace)
                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return fraudMMDetailResult;
        }

        private MMDetailTicketListViewModel GetFraudMMDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, TicketDetailViewModel fraudTicket)
        {
            var mdResult = new MMDetailTicketListViewModel();
            var matchDetailInfo = fraudTicket.MatchDetailInfo;

            if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
            {
                var mmStatic = new MatchMonitorStaticListModel();
                var ticketDetail = fraudTicket.TicketDetails;
                var mergeGroup = fraudTicket.MMDetailMergeGroup;

                var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);

                var custIDs = matchDetailInfo.Custs.Select(x => x.CustID).Distinct();
                var custInfo = GetCustInfoByCustID(custIDs);

                var ticketMergeGroups = fraudTicket.AGroupReasonMerged;

                var transJson = fraudTicket.GetTransByFilterIncludeSuspicous(filter, ticketMergeGroups);

                var ticketDetailGroup = _matchMonitorService.GetTicketDetailByFraudTranIDs(para, filter, transJson);

                custIDs = ticketDetailGroup.TicketDetails.Select(ticket => ticket.CustId).Distinct();

                custInfo = GetCustInfoByCustID(custIDs);

                var mergeInfo = _matchMonitorService.DrawColorFraudMMDetailWithMergeReasonAGroup(matchDetailInfo.VerifiedTrans, custInfo, ticketDetailGroup.TicketDetails, mergeGroup, reasons);

                var mdItemList = MappingRobotStatus(mergeInfo, para.BetTypeID);

                if (para.BetChoiceType == 1)
                {
                    mdResult.ItemList = mdItemList.OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                }
                else
                {
                    mdResult.HomeMatchItemList = mdItemList.Where(x => x.ChoiceOrder == 10).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                    mdResult.AwayMatchItemList = mdItemList.Where(x => x.ChoiceOrder == 20).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                }

                mdResult.RecordsTotal = mdItemList?.Count() ?? 0;
            }

            return mdResult;
        }

        private MMDetailTicketListViewModel GetAllMMDetail(MMDetailInfoViewModel info, MMDetailFilterViewModel filter)
        {
            var para = info.Clone();
            para.TypeOfTickets = MMTicketTypeFromUI.TicketAll;
            var mdResult = new MMDetailTicketListViewModel();
            var ticketDetailGroup = new TicketDetailViewModel();

            try
            {
                if (info.BetChoiceType != 1)
                {
                    var matchDetailInfo = _matchMonitorService.GetMatchMonitorDetail(para);

                    if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
                    {
                        var fraudTicket = GetFraudMMDetailForCache(para);

                        var transJson = _matchMonitorService.GetTransFilterSuspicousMGr(filter, fraudTicket);

                        ticketDetailGroup = GetTicketDetailAll(para, filter, transJson);

                        mdResult.SetLastMark(para);

                        var custIDs = ticketDetailGroup.TicketDetails.Select(ticket => ticket.CustId).Distinct();

                        var custInfo = GetCustInfoByCustID(custIDs);

                        var mdItemList = DrawColorAllMMDetailAndUpdateValue(matchDetailInfo.VerifiedTrans, custInfo, ticketDetailGroup.TicketDetails, para.BetTypeID);

                        mdItemList = _matchMonitorService.MergeFraudToAllMMDetail(mdItemList, fraudTicket?.AGroupReasonMerged);

                        mdResult.HomeMatchItemList = mdItemList?.Where(x => x.ChoiceOrder == 10).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();

                        mdResult.AwayMatchItemList = mdItemList?.Where(x => x.ChoiceOrder == 20).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();

                        mdResult.RecordsTotal = mdItemList?.Count() ?? 0;
                    }
                    else
                    {
                        mdResult = GetTicketsFromMainDb(para, filter);
                    }
                }
                else if (para.TypeOfTickets == MMTicketTypeFromUI.TicketAll)
                {
                    mdResult = GetTicketsFromMainDb(para, filter);
                }

                return mdResult;
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddMessage($"{Region} Get All Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                  + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                  + ex.InnerException.StackTrace)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return mdResult;
        }

        private TicketDetailViewModel GetTicketDetailAll(MMDetailInfoViewModel info, MMDetailFilterViewModel filter, string transJson)
        {
            var ticketDetails = new List<TicketDetailModel>();
            var result = new TicketDetailViewModel();
            var isContinue = true;

            if (filter.CustAmountRM > 0)
            {
                result = _matchMonitorService.GetMainTicketDetail(info, filter, -1, transJson);
                info.SetLastMark(result);
            }
            else
            {
                do
                {
                    result = _matchMonitorService.GetMainTicketDetail(info, filter, detailBatchSize, transJson);

                    result.TicketDetails = result.TicketDetails.OrderByDescending(x => x.SequenceID);

                    info.SetLastMark(result);

                    if (!StopLoopCallDataMMCondition(result) && result.TicketDetails.Count() > 0)
                    {
                        ticketDetails.AddRange(result.TicketDetails);
                    }

                    if (StopLoopCallDataMMCondition(result) || ticketDetails.Count() >= detailBatchSize)
                    {
                        result.TicketDetails = ticketDetails.ToList();

                        isContinue = false;
                    }

                } while (isContinue);
            }

            return result;
        }

        private bool StopLoopCallDataMMCondition(TicketDetailViewModel result)
        {
            return result.MinSequenceId is null && result.MinTransDate is null;
        }

        private IEnumerable<MatchMonitorDetailItemViewModel> MappingRobotStatus(IEnumerable<MatchMonitorDetailItemViewModel> ticket, int betTypeID)
        {
            if (ticket.Any())
            {
                var para = ticket.Select(c => new { TransID = c.TransId, TransDate = c.TransTime, CustID = c.CustID });

                var robotStatus = _matchMonitorService.GetRobotStatus(JsonConvert.SerializeObject(para));

                if (robotStatus.Any())
                {
                    return ticket.GroupJoin(robotStatus,
                               t => t.TransId,
                               r => r.TransID,
                               (t, r) => new { t, r })
                               .SelectMany(
                                    group => group.r.DefaultIfEmpty(),
                                    (g, r) =>
                                    {
                                        var nt = g.t;
                                        nt.RobotType = (r != null) ? r?.RobotType : null;
                                        nt.IsRobot = (r != null) && r.IsRobot;
                                        nt.BetTypeID = betTypeID;
                                        return nt;
                                    });
                }
            }

            return ticket;
        }

        private IEnumerable<MatchMonitorDetailItemViewModel> DrawColorAllMMDetailAndUpdateValue(IEnumerable<MatchMonitorDetailVerifyTransResultModel> verifiedTrans,
            IEnumerable<CustInfoModel> custInfos,
            IEnumerable<TicketDetailModel> tickets,
            int betTypeID)
        {
            var ticketResult = new List<MatchMonitorDetailItemViewModel>();
            var custInfoDictionary = custInfos.ToDictionary(x => x.CustID, x => x);

            if (custInfos == null || tickets == null)
                return ticketResult;

            foreach (var t in tickets)
            {
                var verifiedTransID = verifiedTrans.FirstOrDefault(y => y.VerifiedTransID == t.TransId)?.VerifiedTransID.ToString() ?? string.Empty;
                var nTicket = custInfoDictionary.TryGetValue(t.CustId, out var custInfo) ? new MatchMonitorDetailItemViewModel(t, custInfo) : new MatchMonitorDetailItemViewModel(t);

                nTicket.PICVerified = verifiedTransID;
                nTicket.BetTypeID = betTypeID;
                nTicket.ADetectGroup = "-";
                nTicket.ReasonList = "-";
                nTicket.ReasonIDList = string.Empty;
                nTicket.TypeOfTicket = MMTicketTypeFromUI.TicketAll;

                ticketResult.Add(nTicket);
            }

            return ticketResult;
        }

        private MMDetailTicketListViewModel GetTicketsFromMainDb(MMDetailInfoViewModel para, MMDetailFilterViewModel filter)
        {
            var verifiedTrans = new List<MatchMonitorDetailVerifyTransResultModel>();
            var ticketFromMainDb = new MMDetailTicketListViewModel();

            var transJson = _matchMonitorService.GetTransFilterSuspicousMGr(filter, null);

            var ticketDetailGroup = GetTicketDetailAll(para, filter, transJson);

            var custInfo = GetCustInfoByCustID(ticketDetailGroup.TicketDetails.Select(ticket => ticket.CustId).Distinct());

            var mdItemList = DrawColorAllMMDetailAndUpdateValue(verifiedTrans, custInfo, ticketDetailGroup.TicketDetails, para.BetTypeID);

            if (para.BetChoiceType == 1)
            {
                ticketFromMainDb.ItemList = mdItemList.OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
            }
            else
            {
                ticketFromMainDb.HomeMatchItemList = mdItemList?.Where(x => x.ChoiceOrder == 10).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                ticketFromMainDb.AwayMatchItemList = mdItemList?.Where(x => x.ChoiceOrder == 20).OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
            }

            ticketFromMainDb.RecordsTotal = mdItemList?.Count() ?? 0;
            ticketFromMainDb.SetLastMark(para);

            return ticketFromMainDb;
        }

        private MMDetailFilterViewModel GetMatchMonitorFilterImportNew(MMDetailInfoViewModel para)
        {
            var result = new MMDetailFilterViewModel();

            var removeKeyFraud = para.BuildCacheKey(string.Empty, Region, UserID, Enum.GetName(typeof(MMTicketTypeFromUI), MMTicketTypeFromUI.TicketFraud));
            _memoryCacheService.RemoveKeyContainsValue(removeKeyFraud);

            var removeKeyAll = para.BuildCacheKey(string.Empty, Region, UserID, Enum.GetName(typeof(MMTicketTypeFromUI), MMTicketTypeFromUI.TicketAll));
            _memoryCacheService.RemoveKeyContainsValue(removeKeyAll);

            switch (para.TypeOfTickets)
            {
                case MMTicketTypeFromUI.TicketFraud:
                    {
                        result = GetFraudMMFilter(para);

                        if (para.IsSupportShowAllTickets)
                        {
                            Task.Run(() =>
                            {
                                para.TypeOfTickets = MMTicketTypeFromUI.TicketAll;

                                _ = GetAllMMFilter(para);
                            });
                        }

                        break;
                    }
                case MMTicketTypeFromUI.TicketAll:
                    {
                        para.TypeOfTickets = MMTicketTypeFromUI.TicketFraud;

                        GetFraudMMDetailForCache(para);

                        if (para.IsSupportShowAllTickets)
                        {
                            para.TypeOfTickets = MMTicketTypeFromUI.TicketAll;

                            result = GetAllMMFilter(para);
                        }

                        break;
                    }
            }

            return result;
        }

        private MMDetailFilterViewModel GetFraudMMFilter(MMDetailInfoViewModel para)
        {
            var fraudFilterResult = new MMDetailFilterViewModel();
            para.TypeOfTickets = MMTicketTypeFromUI.TicketFraud;
            var ticketTypeName = Enum.GetName(typeof(MMTicketTypeFromUI), para.TypeOfTickets);

            var key = para.BuildCacheKey(nameof(GetFraudMMFilter), Region, UserID, ticketTypeName);

            lock (key)
            {
                fraudFilterResult = _memoryCacheService.GetValue<MMDetailFilterViewModel>(key);

                if (fraudFilterResult == null)
                {
                    var fraudTicket = GetFraudMMDetailForCache(para);

                    if (fraudTicket != null)
                    {
                        var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);

                        fraudFilterResult = _matchMonitorService.GetFraudFilterInfoDetail(para, fraudTicket, reasons);

                        _memoryCacheService.PushValueAsync(key, fraudFilterResult);
                    }
                }
            }

            fraudFilterResult = fraudFilterResult ?? new MMDetailFilterViewModel();
            fraudFilterResult.IsDisplaySingleGrid = para.BetChoiceType == 1;

            return fraudFilterResult;
        }

        private MMDetailFilterViewModel GetAllMMFilter(MMDetailInfoViewModel para)
        {
            var allFilterResult = new MMDetailFilterViewModel();
            var ticketTypeName = Enum.GetName(typeof(MMTicketTypeFromUI), para.TypeOfTickets);
            var key = para.BuildCacheKey(nameof(GetAllMMFilter), Region, UserID, ticketTypeName);

            try
            {
                allFilterResult = _memoryCacheService.GetValue<MMDetailFilterViewModel>(key);

                if (allFilterResult == null)
                {
                    var fraudTicket = GetFraudMMDetailForCache(para);

                    if (fraudTicket != null)
                    {
                        var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);

                        allFilterResult = _matchMonitorService.GetFraudFilterInfoDetail(para, fraudTicket, reasons);

                        _memoryCacheService.PushValueAsync(key, allFilterResult);
                    }
                    else
                    {
                        allFilterResult = _matchMonitorService.GetAllFilterInfoDetail(para);
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddMessage($"{Region} Get All MM Filter Error: " + ex.Message + ex.StackTrace + "\n"
                     + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                     + ex.InnerException.StackTrace)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                throw;
            }

            allFilterResult = allFilterResult ?? new MMDetailFilterViewModel();
            allFilterResult.IsDisplaySingleGrid = para.BetChoiceType == 1;

            return allFilterResult;
        }

        private IEnumerable<CustInfoModel> GetCustInfoByCustID(IEnumerable<long> custIds)
        {
            var custIdsToString = string.Join(",", custIds);
            if (!string.IsNullOrWhiteSpace(custIdsToString))
            {
                return _matchMonitorService.GetCustInfoByCustID(custIdsToString);
            }

            return Enumerable.Empty<CustInfoModel>();
        }

        private IEnumerable<DangerMonitorBetTypeViewModel> GetSportBettypes()
        {
            var betTypes = GetBetTypeSettings(null, false);

            var filter = new SportBettypeModel
            {
                ProductId = (int)ProductType.SportBook,
                BettypeList = string.Join(",", betTypes.Select(x => x.BetTypeID).Distinct())
            };

            var mainBetTypes = _commonService.GetMainSportBettype(filter);

            return mainBetTypes
                .GroupJoin(betTypes, main => main.Typeid, cts => cts.BetTypeID, (main, ctsGroup) => new { main, ctsGroup })
                .SelectMany(item =>
                    item.ctsGroup.Select(group => new DangerMonitorBetTypeViewModel
                    {
                        SportType = item.main.SportType,
                        SportName = item.main.SportName,
                        BetTypeID = group.BetTypeID,
                        BetTypeName = group.BetTypeNameDisplay,
                        BetID = _commonService.GetBetID(group.BetIDPattern, group.NameDetailsX),
                        BetChoiceType = group.BetChoiceType,
                        BetChoiceHome = group.BetChoiceHome,
                        BetChoiceAway = group.BetChoiceAway,
                        PriorityOrder = group.PriorityOrder,
                        BetIDPattern = group.BetIDPattern
                    }));
        }
        #endregion
    }
}
