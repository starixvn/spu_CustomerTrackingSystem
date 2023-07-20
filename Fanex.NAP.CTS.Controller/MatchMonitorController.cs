namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Common.Extensions;
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : BaseController
    {
        private const string MMDetailTemplate = "~/App_Data/Excel Template/MatchMonitor/Template.xlsx";
        private readonly IMatchMonitorService _matchMonitorService;
        private readonly IAssociationDetectionService _associationDetectionService;
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly string MMDetailRegion = "MMDetail";
        private readonly string FlagRunningTask = "FlagRunningTask";

        public override string BaseControllerName { get => Consts.MenuItemMonitor; }

        public MatchMonitorController()
        {
            _matchMonitorService = new MatchMonitorService();
            _associationDetectionService = new AssociationDetectionService();
            _memoryCacheService = new MemoryCacheService();
            MMDetailRegion = CurrentSession.UserId.ToString();
        }

        #region Main Page
        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.MatchMonitor,
                            NAP.Core.Business.Constant.Policy.Host);

            var userId = CurrentSession.UserId;

            var mmStatic = _matchMonitorService.GetMMStaticList();

            return View(new MatchMonitorViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = userId,
                Reasons = mmStatic.Reasons,
                Sports = mmStatic.Sports
            });
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorRpt, "isSkipTracking")]
        public ActionResult GetMatchMonitor(MatchMonitorSearchViewModel para, bool? isSkipTracking)
        {
            var matchMonitors = Enumerable.Empty<GetMatchMonitorViewModel>();

            try
            {
                if (para != null
                    && !string.IsNullOrEmpty(para.BetTypes)
                    && !string.IsNullOrEmpty(para.MatchSection)
                    && !string.IsNullOrEmpty(para.Sports)
                    && !string.IsNullOrEmpty(para.Reasons)
                    && IsValidDateRange(para.FromDate, para.ToDate))
                {
                    var matchFromCTSInfos = Enumerable.Empty<GetMatchMonitorModel>();

                    var mmStatic = _matchMonitorService.GetMMStaticList();

                    if (para.DateType == 1)
                        matchFromCTSInfos = GetMMByScanDate(para);
                    else
                        matchFromCTSInfos = GetMMByEventDate(para);

                    matchMonitors = matchFromCTSInfos?.Select(x => new GetMatchMonitorViewModel(x, mmStatic)).OrderBy(c => c.MatchSection);

                    if (!string.IsNullOrEmpty(para.Event))
                    {
                        matchMonitors = matchMonitors.Where(x => x.HomeName.ToUpper().Contains(para.Event)
                        || x.AwayName.ToUpper().Contains(para.Event)
                        || x.LeagueName.ToUpper().Contains(para.Event));
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Get Match Monitor Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace);
            }

            return Json(new
            {
                data = matchMonitors
            }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<GetMatchMonitorModel> GetMMByEventDate(MatchMonitorSearchViewModel para)
        {
            var matchFromCTSInfos = Enumerable.Empty<GetMatchMonitorModel>();

            var matchFromMainInfos = _matchMonitorService.GetMatchFromMainInfos(para, string.Empty);

            if (matchFromMainInfos.Any())
            {
                var matchJson = JsonConvert.SerializeObject(matchFromMainInfos);

                matchFromCTSInfos = _matchMonitorService.GetMatchMonitor(para, matchJson);
                matchFromCTSInfos = matchFromCTSInfos.Join(matchFromMainInfos, x => x.MatchID, y => y.MatchId, (x, y) =>
                {
                    x.IsSupportShowAllTickets = y.IsGetDataFromOrigin == 1;
                    return x;
                });
            }

            return matchFromCTSInfos;
        }

        private IEnumerable<GetMatchMonitorModel> GetMMByScanDate(MatchMonitorSearchViewModel para)
        {
            var matchFromCTSInfos = _matchMonitorService.GetMatchMonitor(para, null);

            if (matchFromCTSInfos.Any())
            {
                var matchIDs = string.Join(",", matchFromCTSInfos.Select(x => x.MatchID).Distinct());

                var matchFromMainInfos = _matchMonitorService.GetMatchFromMainInfos(para, matchIDs);

                var newMatches = matchFromCTSInfos.Join(matchFromMainInfos,
                    m1 => m1.MatchID,
                    m2 => m2.MatchId, (m1, m2) => new GetMatchMonitorModel
                    {
                        ID = m1.ID,
                        MatchID = m1.MatchID,
                        Market = m1.Market,
                        HomeID = m1.HomeID,
                        AwayID = m1.AwayID,
                        LeagueID = m1.LeagueID,
                        LeagueName = m1.LeagueName,
                        Sporttype = m1.Sporttype,
                        BettypeID = m1.BettypeID,
                        BetID = m1.BetID,
                        Reason = m1.Reason,
                        IsVerified = m1.IsVerified,
                        VerifiedBy = m1.VerifiedBy,
                        KickOffTime = m2.KickOffTime,
                        EventDate = m2.EventDate,
                        MatchSection = m2.MatchSection,
                        IsSupportShowAllTickets = m2.IsGetDataFromOrigin == 1
                    });

                return newMatches;
            }

            return matchFromCTSInfos;
        }

        #endregion

        #region Detail Page
        [HttpGet]
        public ActionResult Detail(MatchMonitorDetailViewModel para)
        {
            IUserService userService = new UserService();
            var matchGroup = Enumerable.Empty<MatchGroupModel>();
            var matchScore = Enumerable.Empty<MatchScoreModel>();

            try
            {
                if (para != null && para.SportType != (int)MatchMonitorSportType.Basketball)
                {
                    matchScore = _matchMonitorService.GetMatchScore(para.MatchID, para.BetTypeID, para.LiveIndicator, para.BetID, para.Reason) ?? new List<MatchScoreModel>();
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                     + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                     + ex.InnerException.StackTrace);
            }

            ViewBag.EventDateToString = para?.EventDateToString;
            ViewBag.MatchID = para?.MatchID;
            ViewBag.BetTypeID = para?.BetTypeID;
            ViewBag.BettypeName = para?.BettypeName ?? string.Empty;
            ViewBag.BetIDToString = para?.BetID.ToString();
            ViewBag.HomeName = para?.HomeName.ToUpper() ?? string.Empty;
            ViewBag.AwayName = para?.AwayName.ToUpper() ?? string.Empty;
            ViewBag.LiveIndicator = para?.LiveIndicator ?? true;
            ViewBag.Reason = para?.Reason;
            ViewBag.ReasonName = para?.ReasonName ?? string.Empty;
            ViewBag.MatchScore = matchScore?.OrderBy(c => c.ScoreDiff).ThenBy(c => c.LiveHomeScore).ThenBy(c => c.LiveAwayScore);
            ViewBag.LeagueName = para?.LeagueName.ToUpper() ?? string.Empty;
            ViewBag.ReasonGroup = para?.ReasonGroup ?? string.Empty;
            ViewBag.MarketName = para?.MarketName ?? string.Empty;
            ViewBag.BetChoice = para?.BetChoice;
            ViewBag.IsDisableScoreFilter = para?.SportType != null && para.SportType == (int)MatchMonitorSportType.Basketball;
            ViewBag.SportName = para?.SportName ?? string.Empty;
            ViewBag.KOTime = para?.KOTime;
            ViewBag.IsSupportShowAllTickets = para?.IsSupportShowAllTickets;
            ViewBag.IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                                                                        NapSettings.Settings.SiteId,
                                                                        FSFunctionKeys.MatchMonitor,
                                                                        NAP.Core.Business.Constant.Policy.Host);
            return View();
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailRpt)]
        public ActionResult GetMatchMonitorDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter)
        {
            var mdResult = default(MatchMonitorDetailListViewModel);

            switch (para.TypeOfTickets)
            {
                case MatchMonitorQueryType.TicketAll when para.IsSupportShowAllTickets && filter.IsOnlyFraudTicket():
                case MatchMonitorQueryType.TicketFraud:
                    {
                        mdResult = GetFraudMMDetailFromCache(para, para.Sites).Clone();
                        if (filter.IsValidFilterTicketLatest(GetFraudMMFilterFromCache(para)))
                        {
                            mdResult.FilterItemMMDetail(filter, para.Sites);
                        }
                        break;
                    }
                case MatchMonitorQueryType.TicketAll when para.IsSupportShowAllTickets:
                    {
                        filter.IsValidFilterTicketLatest(Task.Run(() => GetAllMMFilterFromCache(para)).Result);
                        mdResult = GetAllMMDetailFromCache(para, filter).Clone();
                        if (filter.ReasonList != null || filter.AGroupList != null)
                        {
                            mdResult.FilterItemMMDetailByReasonAGroup(filter);
                        }
                        break;
                    }
            }

            return Json(new
            {
                data = mdResult,
                recordsFiltered = (mdResult?.AwayMatchItemList?.Count() ?? 0) + (mdResult?.HomeMatchItemList?.Count() ?? 0),
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task<MatchMonitorDetailListViewModel> GetAllMMDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, bool isLoop = false)
        {
            var result = default(MatchMonitorDetailListViewModel);

            var mdItemList = Enumerable.Empty<MatchMonitorDetailItemViewModel>();
            var matchDetailInfo = new MMDetailResultModel();
            var custInfo = Enumerable.Empty<CustInfoModel>();
            var fraudTicketDetail = new MatchMonitorDetailListViewModel();
            var ticketDetailGroup = default(TicketDetailViewModel);

            matchDetailInfo = _matchMonitorService.GetMatchMonitorDetail(para);

            if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
            {
                var tasks = new Task[]
                {
                        Task.Run(() => custInfo = GetCustInfoByCustID(matchDetailInfo.Custs.Select(c => c.CustID).Distinct())),
                        Task.Run(() => fraudTicketDetail = GetFraudMMDetailFromCache(para)),
                        Task.Run(() => ticketDetailGroup = GetAllMMTicketDetail(para, filter, isLoop))
                };

                await Task.WhenAll(tasks);

                var custInfoOthers = GetCustInfoByCustID(ticketDetailGroup.TicketDetails.Select(ticket => ticket.CustId).Distinct().Except(custInfo.Select(x => x.CustID)));

                if (custInfoOthers != null)
                {
                    var meregeCustInfo = custInfo.ToList();
                    meregeCustInfo.AddRange(custInfoOthers);
                    custInfo = meregeCustInfo;
                }

                mdItemList = DrawColorAllMMDetailAndUpdateValue(matchDetailInfo.VerifiedTrans, custInfo, ticketDetailGroup.TicketDetails, para.BetTypeID);
                result = MergeFraudToAllMMDetail(mdItemList, fraudTicketDetail, ticketDetailGroup.MinSequenceIDTotalAll);
            }

            return result;
        }

        private async Task<MatchMonitorDetailListViewModel> GetFraudMMDetailSiteAll(MMDetailInfoViewModel para)
        {
            var mdResult = new MatchMonitorDetailListViewModel();
            var mdItemList = Enumerable.Empty<MatchMonitorDetailItemViewModel>();

            var matchDetailInfo = _matchMonitorService.GetMatchMonitorDetail(para);

            if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
            {
                var mmStatic = new MatchMonitorStaticListModel();
                var custInfo = Enumerable.Empty<CustInfoModel>();
                var mergeGroup = new MMDetailMergeGroupViewModel();
                var ticketDetail = Enumerable.Empty<TicketDetailModel>();

                var tasks = new Task[]
                {
                    Task.Run(() =>
                    {
                        var getTicketDetailAndMergeGroup = GetFraudMMTicketDetail(matchDetailInfo, para);
                        ticketDetail = getTicketDetailAndMergeGroup.TicketDetails;
                        mergeGroup = getTicketDetailAndMergeGroup.MMDetailMergeGroup;
                    }),
                    Task.Run(() => mmStatic = _matchMonitorService.GetMMStaticList()),
                    Task.Run(() => custInfo = GetCustInfoByCustID(matchDetailInfo.Custs.Select(c => c.CustID).Distinct()))
                };

                await Task.WhenAll(tasks);

                var mergeInfo = DrawColorFraudMMDetailWithMergeReasonAGroup(matchDetailInfo.VerifiedTrans, custInfo, ticketDetail, mergeGroup, mmStatic.Reasons);

                mdItemList = MappingRobotStatus(mergeInfo, para.BetTypeID);

                mdResult = new MatchMonitorDetailListViewModel
                {
                    HomeMatchItemList = mdItemList.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.H), StringComparison.OrdinalIgnoreCase)
                        || string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.O), StringComparison.OrdinalIgnoreCase))
                            .OrderByDescending(x => x.TransTime).ToList(),
                    AwayMatchItemList = mdItemList.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.A), StringComparison.OrdinalIgnoreCase)
                        || string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.U), StringComparison.OrdinalIgnoreCase))
                            .OrderByDescending(x => x.TransTime).ToList(),
                    ScoreList = mdItemList.Select(x => x.Score).Distinct().OrderBy(x => x).ToList(),
                    GroupList = mergeGroup.Groups.ToList(),
                    CurrencyList = mdItemList.Select(x =>
                    {
                        var isNotValid = string.IsNullOrEmpty(x.CurrencyName) || x.CurrencyId == null;
                        return new { CurrencyId = isNotValid ? -1 : x.CurrencyId, CurrencyName = isNotValid ? "-" : x.CurrencyName };
                    }).Distinct().OrderBy(x => x.CurrencyName).Select(x => new KeyValuePair<int, string>(x.CurrencyId, x.CurrencyName)).ToList(),
                    HDPList = mdItemList.Select(x => x.BallHeadHDP()).Distinct().OrderBy(x => x).ToList(),
                };

                mdResult.RecordsTotal = (mdResult?.HomeMatchItemList?.Count() ?? 0) + (mdResult?.AwayMatchItemList?.Count() ?? 0);
                mdResult.BindReasons(mdItemList.Select(x => x.ReasonList).Distinct());
            }


            return mdResult;
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

        private TicketDetailViewModel GetFraudMMTicketDetail(MMDetailResultModel matchDetailInfo, MMDetailInfoViewModel para)
        {
            var result = new TicketDetailViewModel();

            var tranIdsDistinct = matchDetailInfo.MatchDetails.Select(c => c.TransID).Distinct();

            result.TicketDetails = _matchMonitorService.GetFraudTicketDetail(para, string.Join(",", tranIdsDistinct));
            var mergeGroup = new MMDetailMergeGroupViewModel();
            var mMDetailBetTeams = matchDetailInfo.MatchDetails.Join(result.TicketDetails,
                m => m.TransID,
                t => t.TransId, (m, t) => new MMDetailBetTeamModel(m, t));
            if (mMDetailBetTeams.Any())
            {
                var ticketMergeGroups = _matchMonitorService.GetMMDetailMergeGroup(mMDetailBetTeams);
                mergeGroup.TicketMergeGroups = ticketMergeGroups;
                mergeGroup.Groups = mergeGroup.GroupByGroupID();
            }

            result.MMDetailMergeGroup = mergeGroup;

            return result;
        }

        private TicketDetailViewModel GetAllMMTicketDetail(MMDetailInfoViewModel info, MMDetailFilterViewModel filter, bool isLoop = false)
        {
            var batchSize = 5000;
            var ticketDetails = new List<TicketDetailModel>();
            var result = new TicketDetailViewModel();

            do
            {
                result = _matchMonitorService.GetAllTicketDetail(info, filter, batchSize);
                result.TicketDetails = result.TicketDetails.OrderByDescending(x => x.SequenceID);

                info.LastScannedSequenceID = result.MinSequenceIDTotalAll;

                if (result.MinSequenceIDTotalAll != null && result.TicketDetails.Count() > 0)
                {
                    ticketDetails.AddRange(result.TicketDetails);
                }


                if (result.MinSequenceIDTotalAll == null || ticketDetails.Count() >= batchSize)
                {
                    result.TicketDetails = ticketDetails.ToList();
                    break;
                }
            } while (isLoop);

            return result;
        }

        [HttpPost]
        public ActionResult GetMatchMonitorTotalTicket(MMDetailInfoViewModel info, MMDetailFilterViewModel filter)
        {
            long recordsTotal = 0;

            switch (info.TypeOfTickets)
            {
                case MatchMonitorQueryType.TicketAll when info.IsSupportShowAllTickets && filter.IsOnlyFraudTicket():
                case MatchMonitorQueryType.TicketFraud:
                    {
                        var mdResult = GetFraudMMDetailFromCache(info, info.Sites).Clone();
                        if (filter.IsValidFilterTicketLatest(GetFraudMMFilterFromCache(info)))
                        {
                            mdResult.FilterItemMMDetail(filter, info.Sites);
                        }
                        recordsTotal = (mdResult?.AwayMatchItemList?.Count() ?? 0) + (mdResult?.HomeMatchItemList?.Count() ?? 0);
                        break;
                    }
                case MatchMonitorQueryType.TicketAll when info.IsSupportShowAllTickets:
                    {
                        filter.IsValidFilterTicketLatest(Task.Run(() => GetAllMMFilterFromCache(info)).Result);
                        recordsTotal = GetTotalAllTicketDetailByFilter(info, filter);
                        break;
                    }
            }


            return Json(new
            {
                recordsTotal = recordsTotal,
            }, JsonRequestBehavior.AllowGet);
        }

        private long GetTotalAllTicketDetailByFilter(MMDetailInfoViewModel info, MMDetailFilterViewModel filter)
        {
            long result = 0;

            var transIdsRemove = new List<string>();

            if (filter.ReasonList != null || filter.AGroupList != null)
            {
                var fraudTicket = GetFraudMMDetailFromCache(info, info.Sites);
                var recordsTotal = fraudTicket.RecordsTotal;

                var filterReasonAGroup = filter.Clone();
                filterReasonAGroup.ReasonList = null;
                filterReasonAGroup.AGroupList = null;

                var fraudTicketRAFilterAll = fraudTicket.Clone();
                fraudTicketRAFilterAll.FilterItemMMDetail(filterReasonAGroup, info.Sites);

                var fraudTicketRAFilter = fraudTicket.Clone();
                fraudTicketRAFilter.FilterItemMMDetail(filter, info.Sites);

                var transIdsAll = new List<string>();
                var transIds = new List<string>();

                transIdsAll.AddRange(fraudTicketRAFilterAll.HomeMatchItemList.Select(x => x.BetID));
                transIdsAll.AddRange(fraudTicketRAFilterAll.AwayMatchItemList.Select(x => x.BetID));
                transIds.AddRange(fraudTicketRAFilter.HomeMatchItemList.Select(x => x.BetID));
                transIds.AddRange(fraudTicketRAFilter.AwayMatchItemList.Select(x => x.BetID));

                transIdsRemove = transIdsAll.Except(transIds).ToList();
            }

            result = (long)_matchMonitorService.GetAllTicketDetail(info, filter, 0, transIdsRemove.Count > 0 ? string.Join(",", transIdsRemove) : null).MinSequenceIDTotalAll;

            return result;
        }

        private MatchMonitorDetailListViewModel GetAllMMDetailFromCache(MMDetailInfoViewModel info, MMDetailFilterViewModel filter)
        {
            var para = info.Clone();
            para.TypeOfTickets = MatchMonitorQueryType.TicketAll;
            var result = new MatchMonitorDetailListViewModel();

            try
            {
                result = Task.Run(() => GetAllMMDetail(para, filter, true)).Result;
            }
            catch (Exception ex)
            {
                LogService.Info("Get All Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                      + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                      + ex.InnerException.StackTrace);
            }

            return result;
        }

        private MatchMonitorDetailListViewModel GetFraudMMDetailFromCache(MMDetailInfoViewModel info, string site = MMSites.All)
        {
            var para = info.Clone();
            para.TypeOfTickets = MatchMonitorQueryType.TicketFraud;

            var result = new MatchMonitorDetailListViewModel();

            var flagKey = para.BuildCacheKeyWithSite(nameof(GetFraudMMDetailFromCache), MMDetailRegion, FlagRunningTask);

            try
            {
                var key = para.BuildCacheKeyWithSite(nameof(GetFraudMMDetailFromCache), MMDetailRegion, site);

                lock (key)
                {
                    var keySiteAll = para.BuildCacheKeyWithSite(nameof(GetFraudMMDetailFromCache), MMDetailRegion, MMSites.All);
                    var fraudMMSiteAll = new MatchMonitorDetailListViewModel();

                    result = _memoryCacheService.GetValue<MatchMonitorDetailListViewModel>(key);

                    if (result == null)
                    {
                        while (true)
                        {
                            if (_memoryCacheService.GetValue<object>(flagKey) == null)
                            {
                                _memoryCacheService.PushValueAsync(flagKey, new { });

                                fraudMMSiteAll = _memoryCacheService.GetValue<MatchMonitorDetailListViewModel>(keySiteAll);

                                if (fraudMMSiteAll == null)
                                {
                                    fraudMMSiteAll = Task.Run(() => GetFraudMMDetailSiteAll(para)).Result;
                                    _ = _memoryCacheService.PushValueAsync(keySiteAll, fraudMMSiteAll);
                                }

                                result = fraudMMSiteAll.Clone();
                                if (site != MMSites.All)
                                {
                                    result.FilterItemMMDetailBySite(site);
                                    _ = _memoryCacheService.PushValueAsync(key, result);
                                }

                                _memoryCacheService.RemoveValueAsync(flagKey);

                                break;
                            }
                            else
                            {
                                Thread.Sleep(2000);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _memoryCacheService.RemoveValueAsync(flagKey);
                LogService.Info("Get Fraud Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                      + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                      + ex.InnerException.StackTrace);
            }

            return result;
        }

        [HttpPost]
        public ActionResult GetMatchMonitorFilter(MMDetailInfoViewModel para)
        {
            var result = new MMDetailFilterViewModel();

            try
            {
                switch (para.TypeOfTickets)
                {
                    case MatchMonitorQueryType.TicketAll when para.IsSupportShowAllTickets:
                        {
                            result = Task.Run(() => GetAllMMFilterFromCache(para, para.Sites, false)).Result;
                            break;
                        }
                    case MatchMonitorQueryType.TicketFraud when !para.IsRefresh:
                        {
                            result = GetFraudMMFilterFromCache(para, para.Sites);
                            break;
                        }
                    case MatchMonitorQueryType.TicketFraud when para.IsRefresh:
                        {
                            var removeKey = $"{MMDetailRegion}_{para.EventDateToString}_{para.MatchID}_{para.BetTypeID}_{para.BetID}_{para.LiveIndicator}";
                            _memoryCacheService.RemoveKeyContainsValue(removeKey);
                            result = GetFraudMMFilterFromCache(para, para.Sites);

                            if (para.IsSupportShowAllTickets)
                            {
                                Task.Run(() =>
                                {
                                    _ = GetAllMMFilterFromCache(para);
                                });
                            }

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Get Match Monitor Filter Error: " + ex.Message + ex.StackTrace + "\n"
                     + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                     + ex.InnerException.StackTrace);
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        private MMDetailFilterViewModel GetFraudMMFilterFromCache(MMDetailInfoViewModel info, string site = MMSites.All)
        {
            var para = info.Clone();
            para.TypeOfTickets = MatchMonitorQueryType.TicketFraud;

            var key = para.BuildCacheKeyWithSite(nameof(GetFraudMMFilterFromCache), MMDetailRegion, site);
            var result = new MMDetailFilterViewModel();

            lock (key)
            {

                result = _memoryCacheService.GetValue<MMDetailFilterViewModel>(key);

                if (result == null)
                {
                    result = GetFraudMMDetailFromCache(para, site).Clone();

                    _memoryCacheService.PushValueAsync(key, result);
                }
            }

            return result;
        }

        private async Task<MMDetailFilterViewModel> GetAllMMFilterFromCache(MMDetailInfoViewModel para, string site = MMSites.All, bool isCache = true)
        {
            var keyWithSite = para.BuildCacheKeyWithSite(nameof(GetAllMMFilterFromCache), MMDetailRegion, site);
            var key = para.BuildCacheKey(nameof(GetAllMMFilterFromCache), MMDetailRegion);
            var flagKey = para.BuildCacheKeyWithSite(nameof(GetAllMMFilterFromCache), MMDetailRegion, FlagRunningTask);

            var result = new MMDetailFilterViewModel();
            var mmticketDetailInfo = default(MMTicketDetailInfoModel);

            try
            {
                while (true)
                {
                    if (_memoryCacheService.GetValue<object>(flagKey) == null)
                    {
                        await _memoryCacheService.PushValueAsync(flagKey, new { });
                        result = _memoryCacheService.GetValue<MMDetailFilterViewModel>(keyWithSite);

                        if (result == null)
                        {
                            mmticketDetailInfo = _memoryCacheService.GetValue<MMTicketDetailInfoModel>(key);
                            if (mmticketDetailInfo == null)
                            {
                                mmticketDetailInfo = _matchMonitorService.GetMMTicketDetailInfo(para);
                                mmticketDetailInfo.FilterDetail.ScoreList = null;
                                if (isCache) await _memoryCacheService.PushValueAsync(key, mmticketDetailInfo);
                            }

                            var fraudTicketSiteAll = GetFraudMMFilterFromCache(para, MMSites.All);
                            var fraudTicketDetail = site != MMSites.All ? GetFraudMMFilterFromCache(para, site) : fraudTicketSiteAll;
                            result = new MMDetailFilterViewModel(mmticketDetailInfo, fraudTicketDetail, site);


                            if (isCache)
                            {
                                _ = Task.Run(() =>
                                {
                                    var keySite = para.BuildCacheKeyWithSite(nameof(GetAllMMFilterFromCache), MMDetailRegion, MMSites.All);
                                    _memoryCacheService.PushValueAsync(keySite, new MMDetailFilterViewModel(mmticketDetailInfo, fraudTicketSiteAll, MMSites.All));
                                });
                                _ = Task.Run(() =>
                                {
                                    var keySite = para.BuildCacheKeyWithSite(nameof(GetAllMMFilterFromCache), MMDetailRegion, MMSites.Licensee);
                                    var fraudTicketDetailSite = GetFraudMMFilterFromCache(para, MMSites.Licensee);
                                    _memoryCacheService.PushValueAsync(keySite, new MMDetailFilterViewModel(mmticketDetailInfo, fraudTicketDetailSite, MMSites.Licensee));
                                });
                                _ = Task.Run(() =>
                                {
                                    var keySite = para.BuildCacheKeyWithSite(nameof(GetAllMMFilterFromCache), MMDetailRegion, MMSites.Credit);
                                    var fraudTicketDetailSite = GetFraudMMFilterFromCache(para, MMSites.Credit);
                                    _memoryCacheService.PushValueAsync(keySite, new MMDetailFilterViewModel(mmticketDetailInfo, fraudTicketDetailSite, MMSites.Credit));
                                });
                            }
                        }

                        await _memoryCacheService.RemoveValueAsync(flagKey);

                        break;
                    }
                    else
                    {
                        await Task.Delay(2000);
                    }
                }
            }
            catch (Exception ex)
            {
                await _memoryCacheService.RemoveValueAsync(flagKey);
                LogService.Info("Get Match Monitor Filter Error: " + ex.Message + ex.StackTrace + "\n"
                     + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                     + ex.InnerException.StackTrace);
            }

            return result;
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailManualDetectRpt)]
        public ActionResult DetectGroupAssWithOptions(DetectGroupAssFilterViewModel model)
        {
            var result = new DetectGroupAssociationListViewModel();
            var mergeGroupInfo = new MergeGroupInfoModel { CTSCustIDs = model.CTSCustIDs };

            var custInfo = _matchMonitorService.GetCustInfoByCustID(model.CustIDs);

            Parallel.Invoke(() =>
            {
                var detectAssByIP = model.IsOptIPLast3Days ? _associationDetectionService.DetectAssociationByIP(model.CustIDs) : Enumerable.Empty<DetectAssociationBySharedIPModel>();

                mergeGroupInfo.AssociationByIPJson = this.GetAssociationCustByIP(detectAssByIP, custInfo);
            }, () =>
            {
                var detectAssByAssMatches = model.IsOpt3MatchesLast7Days ? _associationDetectionService.DetectAssociationBySharedMatches(model.CustIDs) : Enumerable.Empty<DetectAssociationBySharedIPMatchesModel>();

                mergeGroupInfo.AssociationByMatchesJson = this.GetAssociationCustByMatch(detectAssByAssMatches, custInfo);
            },
            () =>
            {
                if (model.IsOptBP || model.IsOptDevice || model.IsOptIP)
                {
                    var assByPrimary = _associationDetectionService.DetectAssociationByPrimary(new DetectAssociationByPrimaryFilterModel
                    {
                        CTSCustIDs = model.CTSCustIDs,
                        IsBP = model.IsOptBP,
                        IsDevice = model.IsOptDevice,
                        IsIP = model.IsOptIP
                    });
                    mergeGroupInfo.AssociationByPrimaryJson = JsonConvert.SerializeObject(assByPrimary);
                }
            }, () =>
            {
                var assByAgent = Enumerable.Empty<DetectAssociationByPrimaryModel>();

                if (model.IsOptAgent)
                    assByAgent = _associationDetectionService.DetectAssociationByAgent(model.CTSCustIDs);

                mergeGroupInfo.AssociationByAgentJson = JsonConvert.SerializeObject(assByAgent);
            });

            var data = _associationDetectionService.DetectGroupDetectAssociation(mergeGroupInfo).Select(x => new DetectGroupAssociationItemViewModel(x));

            var assRearrangeGroup = this.RearrangeAssGroup(data, true);

            result = new DetectGroupAssociationListViewModel
            {
                ItemList = assRearrangeGroup
            };

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailVerifyTransRpt)]
        public ActionResult VerifyTransMMDetail(MatchMonitorDetailVerifyTransParaModel para)
        {
            var result = new DBResult();

            try
            {
                if (para != null && !string.IsNullOrEmpty(para.TranIds))
                {
                    para.VerifiedBy = CurrentSession.UserId;

                    _matchMonitorService.VerifyTransMatchMonitorDetail(para);
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Verify Trans MM Detail Error: " + ex.Message + ex.StackTrace + "\n"
                      + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                      + ex.InnerException.StackTrace);

                result.ErrorCode = -1;
                result.ErrorMsg = ex.Message;
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMMFilterBetTypeBySportID(string sportIDs)
        {
            var result = Enumerable.Empty<MatchMonitorBetTypeViewModel>();
            var sportIds = sportIDs.Split(',').Select(x => Convert.ToInt32(x));

            if (sportIds.Any())
            {
                var data = _matchMonitorService.GetMMStaticList();

                if (data.BetTypes != null && data.BetTypes.Any())
                {
                    result = data.BetTypes.Join(sportIds,
                      x1 => x1.SportID,
                      x2 => x2,
                      (x1, x2) => new MatchMonitorBetTypeViewModel
                      {
                          BetTypeID = x1.BetTypeID,
                          BetTypeName = x1.BetTypeName,
                          BetID = x1.BetID,
                          SportID = x1.SportID,
                          IsSelected = true
                      }).GroupBy(g => new { g.BetTypeID, g.BetID })
                      .Select(g => g.First());
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Export to Excel
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailExportToExcelRpt)]
        public ActionResult ExportToExcelMMDetail(FormCollection from)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var matchInfo = serializer.Deserialize<ExExlMatchInfoViewModel>(from["match"]);
            var homeMItems = serializer.Deserialize<List<ExExcelTicketInfoViewModel>>(from["homes"]);
            var awayMItems = serializer.Deserialize<List<ExExcelTicketInfoViewModel>>(from["aways"]);

            var templatePath = Server.MapPath(MMDetailTemplate);

            var fileName = string.Format("MM_{0}_{1}_{2}.xlsx", matchInfo.MatchName, matchInfo.BetTypeName, matchInfo.EventDateStr);

            var excelFile = _matchMonitorService.ExportExcelMMDetail(homeMItems, awayMItems, fileName, templatePath);

            return File(excelFile, "application/vnd.ms-excel", fileName);
        }

        #endregion

        #region Private Methods

        private bool IsValidDateRange(DateTime? fromDate, DateTime? toDate)
        {
            if (toDate != null && fromDate != null)
            {
                TimeSpan t = (toDate?.Date - fromDate?.Date) ?? TimeSpan.MaxValue;
                return t.TotalDays <= 6;
            }

            return false;
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

            tickets.ForEach(t =>
            {
                var verifiedTransID = verifiedTrans.FirstOrDefault(y => y.VerifiedTransID == t.TransId)?.VerifiedTransID.ToString() ?? string.Empty;
                var nTicket = custInfoDictionary.TryGetValue(t.CustId, out var custInfo) ? new MatchMonitorDetailItemViewModel(t, custInfo) : new MatchMonitorDetailItemViewModel(t);

                nTicket.PICVerified = verifiedTransID;
                nTicket.BetTypeID = betTypeID;
                nTicket.ADetectGroup = "-";
                nTicket.ReasonList = "-";
                nTicket.TypeOfTicket = MatchMonitorQueryType.TicketAll;

                ticketResult.Add(nTicket);
            });

            return ticketResult;
        }

        public MatchMonitorDetailListViewModel MergeFraudToAllMMDetail(IEnumerable<MatchMonitorDetailItemViewModel> mdItemList, MatchMonitorDetailListViewModel fraudMMDetailModel, long? lastSequenceID)
        {
            var fraudHomeMatchItemDictionary = fraudMMDetailModel?.HomeMatchItemList?.ToDictionary(x => x.TransId, x => x);
            var fraudAwayMatchItemDictionary = fraudMMDetailModel?.AwayMatchItemList?.ToDictionary(x => x.TransId, x => x);

            return new MatchMonitorDetailListViewModel
            {
                HomeMatchItemList = mdItemList.Where(c => c.FilterByHomeAway(new string[] { nameof(MatchMonitorBetChoiceId.H), nameof(MatchMonitorBetChoiceId.O) }))
                    .Select(x => x.MappingFraudTicketToAllTicket(fraudHomeMatchItemDictionary)).ToList(),
                AwayMatchItemList = mdItemList.Where(c => c.FilterByHomeAway(new string[] { nameof(MatchMonitorBetChoiceId.A), nameof(MatchMonitorBetChoiceId.U) }))
                    .Select(x => x.MappingFraudTicketToAllTicket(fraudAwayMatchItemDictionary)).ToList(),
                LastSequenceID = lastSequenceID
            };
        }

        private IEnumerable<MatchMonitorDetailItemViewModel> DrawColorFraudMMDetailWithMergeReasonAGroup(IEnumerable<MatchMonitorDetailVerifyTransResultModel> verifiedTrans,
            IEnumerable<CustInfoModel> custInfo,
            IEnumerable<TicketDetailModel> ticket,
            MMDetailMergeGroupViewModel mergeGroup,
            IEnumerable<MatchMonitorReasonModel> reasons)
        {
            var ticketResult = Enumerable.Empty<MatchMonitorDetailItemViewModel>();

            if (custInfo == null || ticket == null || mergeGroup.TicketMergeGroups == null)
                return ticketResult;

            ticketResult = mergeGroup.TicketMergeGroups.Join(ticket, tg => tg.TransID, t => t.TransId,
                (tg, t) => new { tg = tg, t = t })
                .Join(custInfo, ttg => ttg.t.CustId, c => c.CustID,
                (ttg, c) => new { ttg = ttg, c = c }).Select(x =>
                {
                    var nTicket = new MatchMonitorDetailItemViewModel(x.ttg.tg.MergeGroupID, x.ttg.t, x.c);
                    var verifiedTransID = verifiedTrans.FirstOrDefault(y => y.VerifiedTransID == x.ttg.t.TransId)?.VerifiedTransID.ToString() ?? string.Empty;
                    var group = mergeGroup.Groups.FirstOrDefault(y => y.GroupID == x.ttg.tg.MergeGroupID);

                    nTicket.PICVerified = verifiedTransID;
                    nTicket.ReasonList = GetReasonName(reasons, x.ttg.tg.ReasonList);
                    nTicket.ADetectGroupID = group?.GroupID ?? 0;
                    nTicket.ADetectGroup = group?.GroupName ?? "-";

                    return nTicket;
                });

            return ticketResult;
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

        private string GetReasonName(IEnumerable<MatchMonitorReasonModel> reasonList, string reasonNameList)
        {
            if (!string.IsNullOrEmpty(reasonNameList))
            {
                var arrReasons = reasonNameList.Split(',').Select(x => Convert.ToInt32(x));
                var reasonNames = reasonList.Join(arrReasons,
                    r1 => r1.ReasonID,
                    r2 => r2,
                    (r1, r2) => r1.ReasonName);

                return string.Join(",", reasonNames);
            }

            return string.Empty;
        }

        private string GetAssociationCustByIP(IEnumerable<DetectAssociationBySharedIPModel> detectAss, IEnumerable<CustInfoModel> custInfos)
        {
            var assCustBySharedIP = detectAss.Select(x =>
            {
                var fromCTSCust = custInfos.FirstOrDefault(y => y.CustID.Equals(x.CustID1)) ?? new CustInfoModel();
                var toCTSCust = custInfos.FirstOrDefault(y => y.CustID.Equals(x.CustID2)) ?? new CustInfoModel();

                return new AssociationCustModel
                {
                    FromCustID = x.CustID1,
                    FromCTSCustID = fromCTSCust.CTSCustID,
                    ToCustID = x.CustID2,
                    ToCTSCustID = toCTSCust.CTSCustID
                };
            }).Where(x => x.FromCTSCustID != 0 && x.ToCTSCustID != 0);

            return JsonConvert.SerializeObject(assCustBySharedIP);
        }

        private string GetAssociationCustByMatch(IEnumerable<DetectAssociationBySharedIPMatchesModel> detectAss, IEnumerable<CustInfoModel> custInfos)
        {
            var assCustBySharedIP = detectAss.Select(x =>
            {
                var fromCTSCust = custInfos.FirstOrDefault(y => y.CustID.Equals(x.CustID1)) ?? new CustInfoModel();
                var toCTSCust = custInfos.FirstOrDefault(y => y.CustID.Equals(x.CustID2)) ?? new CustInfoModel();

                return new AssociationCustModel
                {
                    FromCustID = x.CustID1,
                    FromCTSCustID = fromCTSCust.CTSCustID,
                    ToCustID = x.CustID2,
                    ToCTSCustID = toCTSCust.CTSCustID
                };
            }).Where(x => x.FromCTSCustID != 0 && x.ToCTSCustID != 0);

            return JsonConvert.SerializeObject(assCustBySharedIP);
        }

        private IEnumerable<DetectGroupAssociationItemViewModel> RearrangeAssGroup(IEnumerable<DetectGroupAssociationItemViewModel> assGroupList, bool isDisableNone)
        {
            var result = new List<DetectGroupAssociationItemViewModel>();

            if (isDisableNone)
                assGroupList = assGroupList.Where(x => x.GroupID != 0);

            if (assGroupList.Any())
            {
                var grouping = assGroupList
                    .GroupBy(x => x.GroupID, (key, g) => new { GroupID = key, Items = g.ToList() })
                    .OrderBy(x => x.GroupID);

                var idx = 1;

                foreach (var group in grouping)
                {
                    var groupID = group.GroupID;
                    var groupName = group.GroupID.ToString();

                    if (groupID.HasValue)
                    {
                        if (groupID == 0)
                        {
                            groupID = -1;
                            groupName = "None";
                        }
                        else
                        {
                            groupID = idx;
                            groupName = idx.ToString();
                            idx += 1;
                        }
                    }

                    result.AddRange(group.Items.Select(x =>
                    {
                        x.GroupID = groupID;
                        x.GroupName = groupName;

                        return x;
                    }));
                }
            }

            return result;
        }
        #endregion Private Methods

    }
}