namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.Common;
    using Fanex.NAP.CTS.ViewModel.AssociationDetection;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.DetectGroup;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : BaseController
    {
        private const string MMDetailDoubleGridTemplate = "~/App_Data/Excel Template/MatchMonitor/DoubleGridTemplate.xlsx";
        private const string MMDetailSingleGridTemplate = "~/App_Data/Excel Template/MatchMonitor/SingleGridTemplate.xlsx";

        private readonly IMatchMonitorService _matchMonitorService;
        private readonly IAssociationDetectionService _associationDetectionService;
        private readonly ICommonService _commonService;
        private readonly IMemoryCacheService _memoryCacheService;

        private readonly string UserID = "MMDetailUserID";
        private readonly string Region = "MMDetailRegion";
        private readonly string[] PropertyNameOfSummaryHomeList = { nameof(GetMatchMonitorViewModel.HomeTotalAmount), nameof(GetMatchMonitorViewModel.HomeBetCount) };
        private readonly string[] PropertyNameOfSummaryAwayList = { nameof(GetMatchMonitorViewModel.AwayTotalAmount), nameof(GetMatchMonitorViewModel.AwayBetCount) };

        public override string BaseControllerName { get => Consts.MenuItemMonitor; }

        public MatchMonitorController()
        {
            _matchMonitorService = new MatchMonitorService(MMQueryType.MMReport);
            _commonService = new CommonService();
            _associationDetectionService = new AssociationDetectionService();
            _memoryCacheService = new MemoryCacheService();
            UserID = CurrentSession.UserId.ToString();
        }

        #region Main Page
        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.MatchMonitor,
                            NAP.Core.Business.Constant.Policy.Host);          

            var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true)
                .Select(x => new ReasonStaticListViewModel { ReasonID = x.ItemValue, ReasonName = x.ItemNameDisplay, IsSelected = x.ItemDefaultValue == "1" })
                .OrderBy(x => x.ReasonName);

            var sports = _commonService.GetStaticLists((int)StaticListID.Sport, true)
                .Select(x => new SportStaticListViewModel { SportID = x.ItemValue, SportName = x.ItemNameDisplay, IsSelected = x.ItemDefaultValue == "1" });

            return View(new MatchMonitorViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = CurrentSession.UserId,
                Reasons = reasons,
                Sports = sports
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

                    if (para.DateType == 1)
                        matchFromCTSInfos = GetMMByScanDate(para);
                    else
                        matchFromCTSInfos = GetMMByEventDate(para);

                    var mmDictionaryTicketsSummarys = GetMMFraudTicketsSummary(matchFromCTSInfos);

                    matchMonitors = MergeMatchMonitorWithMMStaticAndTicketsSummary(matchFromCTSInfos, mmDictionaryTicketsSummarys);

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
                var logMessage = new LogMessage(ex.Message)
                    .AddMessage($"{Region} Get Match Monitor Error: " + ex.Message + ex.StackTrace + "\n"
                     + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                     + ex.InnerException.StackTrace)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(new
            {
                data = matchMonitors
            }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<GetMatchMonitorViewModel> MergeMatchMonitorWithMMStaticAndTicketsSummary(IEnumerable<GetMatchMonitorModel> matchFromCTSInfos,
            Dictionary<string, MMFraudTicketsSummaryViewModel> mmDictionaryTicketsSummarys)
        {
            var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);
            var bettypeSettings = GetBetTypeSettings(null, true);

            return
                matchFromCTSInfos?
                    .Select(x =>
                        {
                            var result = new GetMatchMonitorViewModel(x, reasons);
                            var betChoiceHome = bettypeSettings.FirstOrDefault(y => y.BetTypeID == result.BettypeID)?.BetChoiceHome;
                            var betChoiceAway = bettypeSettings.FirstOrDefault(y => y.BetTypeID == result.BettypeID)?.BetChoiceAway;

                            if (mmDictionaryTicketsSummarys != null)
                            {
                                SetValueMMTicketsSummaryFromDictionary(result, betChoiceHome, mmDictionaryTicketsSummarys, PropertyNameOfSummaryHomeList);

                                SetValueMMTicketsSummaryFromDictionary(result, betChoiceAway, mmDictionaryTicketsSummarys, PropertyNameOfSummaryAwayList);
                            }

                            return result;
                        })
                    .OrderBy(c => c.MatchSection);
        }

        private void SetValueMMTicketsSummaryFromDictionary(GetMatchMonitorViewModel result, string betChoice,
            Dictionary<string, MMFraudTicketsSummaryViewModel> mmDictionaryTicketsSummarys, string[] propertyNameOfSummaryList)
        {
            if (mmDictionaryTicketsSummarys.TryGetValue(result.BuildTicketsSummaryDictionaryKey(betChoice), out var mmFraudTicketsSummary))
            {
                result.SetPropertyValue(typeof(GetMatchMonitorViewModel).GetProperty(propertyNameOfSummaryList[0]), mmFraudTicketsSummary.Turnover);

                result.SetPropertyValue(typeof(GetMatchMonitorViewModel).GetProperty(propertyNameOfSummaryList[1]), mmFraudTicketsSummary.BetCount);
            }
        }

        private Dictionary<string, MMFraudTicketsSummaryViewModel> GetMMFraudTicketsSummary(IEnumerable<GetMatchMonitorModel> matchFromCTSInfos)
        {
            var mmFraudTicketsSummaryModel =
                matchFromCTSInfos
                    .GroupBy(x => new { x.BetID, x.BettypeID, x.DataSource, x.Market, x.MatchID })
                    .Select(x => new MMFraudTicketsSummaryModel
                    {
                        BetID = x.Key.BetID,
                        BettypeID = x.Key.BettypeID,
                        Market = x.Key.Market,
                        MatchID = x.Key.MatchID,
                        DataSource = x.Key.DataSource,
                        FraudTrans = String.Join(",", x.Select(y => y.FraudTrans)),
                    });

            return _matchMonitorService.GetMMFraudTicketsSummary(mmFraudTicketsSummaryModel)?.ToDictionary(x => x.BuildDictionaryKey());
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
                    x.DataSource = y.DataSource;
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
                        IsSupportShowAllTickets = m2.IsGetDataFromOrigin == 1,
                        DataSource = m2.DataSource,
                        FraudTrans = m1.FraudTrans,
                        BettypeNameDisplay = m1.BettypeNameDisplay,
                        DetailsX = m1.DetailsX,
                        DetailsY = m1.DetailsY
                    });

                return newMatches;
            }

            return matchFromCTSInfos;
        }

        [HttpPost]
        public ActionResult GetMMFilterBetTypeBySportID(string sportIDs)
        {
            return Json(GetBetTypeSettings(sportIDs, true), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<MatchMonitorBetTypeViewModel> GetBetTypeSettings(string sportIDs, bool isFilterMMDetection)
        {
            var paras = new BetTypeSettingParaModel
            {
                FunctionID = (int)BetTypeSettingsFunctionID.MMBetType,
                BettypeIDList = null,
                SportTypeIDList = sportIDs,
                IsFilterMMDetection = isFilterMMDetection,
                IsShowDetailsX = true,
                Status = 1
            };

            return _commonService.GetBetTypeSettings(paras).Select(x => new MatchMonitorBetTypeViewModel
            {
                SportID = x.SportTypeID,
                BetTypeID = x.BetTypeID,
                BetTypeName = x.BetTypeNameDisplay,
                BetID = _commonService.GetBetID(x.BetIDPattern, x.NameDetailsX),
                BetIDPattern = x.BetIDPattern,
                BetChoiceType = x.BetChoiceType,
                BetChoiceHome = x.BetChoiceHome,
                BetChoiceAway = x.BetChoiceAway,
                IsSupportSuspicious = x.IsMMDetection,
                IsSelected = true,
                NameDetailsX = x.NameDetailsX
            }).GroupBy(g => new { g.BetTypeID, g.BetID }).Select(g => g.First());
        }

        #endregion

        #region Detail Page
        [HttpGet]
        public ActionResult Detail(MatchMonitorDetailViewModel para)
        {
            IUserService userService = new UserService();

            try
            {
                ViewBag.GroupCCList = _commonService.GetGroupCustomerClass();
                ViewBag.EventDateToString = para?.EventDateToString;
                ViewBag.MatchID = para?.MatchID;
                ViewBag.BetTypeID = para?.BetTypeID;
                ViewBag.BettypeName = para?.BettypeName ?? string.Empty;
                ViewBag.BetIDToString = para?.BetID.ToString();
                ViewBag.BetTypeSettings = GetBetTypeSettings(para.SportType.ToString(), false);
                ViewBag.MapX = para?.MX;
                ViewBag.MapYY = para?.MY;
                ViewBag.HomeName = para?.HomeName.ToUpper() ?? string.Empty;
                ViewBag.AwayName = para?.AwayName.ToUpper() ?? string.Empty;
                ViewBag.LiveIndicator = para?.LiveIndicator ?? true;
                ViewBag.Reason = para?.Reason;
                ViewBag.ReasonName = para?.ReasonName ?? string.Empty;
                ViewBag.LeagueName = para?.LeagueName.ToUpper() ?? string.Empty;
                ViewBag.ReasonGroup = para?.ReasonGroup ?? string.Empty;
                ViewBag.MarketName = para?.MarketName ?? string.Empty;
                ViewBag.BetChoice = para?.BetChoice;
                ViewBag.IsDisableScoreFilter = para?.SportType != null && para.SportType == (int)MatchMonitorSportType.Basketball;
                ViewBag.SportType = para?.SportType ?? 0;
                ViewBag.SportName = para?.SportName ?? string.Empty;
                ViewBag.KOTime = para?.KOTime;
                ViewBag.IsSupportShowAllTickets = para?.IsSupportShowAllTickets;
                ViewBag.IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                                                                            NapSettings.Settings.SiteId,
                                                                            FSFunctionKeys.MatchMonitor,
                                                                            NAP.Core.Business.Constant.Policy.Host);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddMessage($"{Region} Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return View();
        }

        #region Detail Filter
        [HttpPost]
        public ActionResult GetMatchMonitorFilter(MMDetailInfoViewModel para)
        {
            var result = new MMDetailFilterViewModel();

            try
            {
                if (para.IsImportNewTicket)
                {
                    result = GetMatchMonitorFilterImportNew(para);
                }
                else
                {
                    if (para.TypeOfTickets == MMTypeOfTicket.TicketFraud)
                    {
                        result = GetFraudMMFilter(para);
                    }
                    else if (para.IsSupportShowAllTickets)
                    {
                        result = GetAllMMFilter(para);
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddMessage($"{Region} Get Match Monitor Filter Error: " + ex.Message + ex.StackTrace + "\n"
                   + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                   + ex.InnerException.StackTrace)
                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        private MMDetailFilterViewModel GetMatchMonitorFilterImportNew(MMDetailInfoViewModel para)
        {
            var result = new MMDetailFilterViewModel();

            var removeKeyFraud = para.BuildCacheKey(string.Empty, Region, UserID, Enum.GetName(typeof(MMTypeOfTicket), MMTypeOfTicket.TicketFraud));
            _memoryCacheService.RemoveKeyContainsValue(removeKeyFraud);

            var removeKeyAll = para.BuildCacheKey(string.Empty, Region, UserID, Enum.GetName(typeof(MMTypeOfTicket), MMTypeOfTicket.TicketAll));
            _memoryCacheService.RemoveKeyContainsValue(removeKeyAll);

            switch (para.TypeOfTickets)
            {
                case MMTypeOfTicket.TicketFraud:
                    {
                        result = GetFraudMMFilter(para);

                        if (para.IsSupportShowAllTickets)
                        {
                            Task.Run(() =>
                            {
                                para.TypeOfTickets = MMTypeOfTicket.TicketAll;

                                _ = GetAllMMFilter(para);
                            });
                        }

                        break;
                    }
                case MMTypeOfTicket.TicketAll:
                    {
                        para.TypeOfTickets = MMTypeOfTicket.TicketFraud;

                        GetFraudMMDetailForCache(para);

                        if (para.IsSupportShowAllTickets)
                        {
                            para.TypeOfTickets = MMTypeOfTicket.TicketAll;

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
            para.TypeOfTickets = MMTypeOfTicket.TicketFraud;
            var ticketTypeName = Enum.GetName(typeof(MMTypeOfTicket), para.TypeOfTickets);

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
            var ticketTypeName = Enum.GetName(typeof(MMTypeOfTicket), para.TypeOfTickets);
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

        #endregion Detail Filter

        #region Detail Total Tickets
        [HttpPost]
        public ActionResult GetMatchMonitorTotalTicket(MMDetailInfoViewModel info, MMDetailFilterViewModel filter)
        {
            long recordsTotal = 0;

            try
            {
                var fraudTicket = GetFraudMMDetailForCache(info);

                var transJson = _matchMonitorService.GetTransFilterSuspicousMGr(filter, fraudTicket);

                recordsTotal = _matchMonitorService.GetTotalTicketDetail(info, filter, 0, transJson);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddMessage($"{Region} Get Match Monitor Total Ticket Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(new
            {
                recordsTotal = recordsTotal,
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion Detail Total Tickets

        #region Detail Tickets
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailRpt)]
        public ActionResult GetMatchMonitorDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter)
        {
            var mdResult = default(MMDetailTicketListViewModel);

            try
            {
                switch (para.TypeOfTickets)
                {
                    case MMTypeOfTicket.TicketFraud:
                        {
                            var fraudTicket = GetFraudMMDetailForCache(para);

                            if (fraudTicket != null)
                            {
                                mdResult = GetFraudMMDetail(para, filter, fraudTicket);
                            }

                            break;
                        }
                    case MMTypeOfTicket.TicketAll when para.IsSupportShowAllTickets:
                        {
                            var internalCurrencyIDs = new List<int> { 20, 27, 28 };

                            mdResult = GetAllMMDetail(para, filter);

                            if (mdResult != null)
                            {
                                var ctsCustIDHomeIsZero = mdResult.HomeMatchItemList.Where(x => x.CTSCustID == 0 && !internalCurrencyIDs.Any(c => c == x.CurrencyId)).Select(x => x);
                                var ctsCustIDAwayIsZero = mdResult.AwayMatchItemList.Where(x => x.CTSCustID == 0 && !internalCurrencyIDs.Any(c => c == x.CurrencyId)).Select(x => x);

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
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddMessage($"{Region} Get Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(new
            {
                data = mdResult,
                recordsFiltered = (mdResult?.RecordsTotal ?? 0)
            }, JsonRequestBehavior.AllowGet);
        }

        private MMDetailTicketListViewModel GetAllMMDetail(MMDetailInfoViewModel info, MMDetailFilterViewModel filter)
        {
            var para = info.Clone();
            para.TypeOfTickets = MMTypeOfTicket.TicketAll;
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

                        ticketDetailGroup = _matchMonitorService.GetTicketDetailAll(para, filter, transJson);

                        mdResult.LastSequenceID = para.LastScannedSequenceID;

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
                else if (para.TypeOfTickets == MMTypeOfTicket.TicketAll)
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

        private TicketDetailViewModel GetFraudMMDetailForCache(MMDetailInfoViewModel filter)
        {
            var para = filter.Clone();
            var fraudMMDetailResult = new TicketDetailViewModel();
            var ticketTypeName = Enum.GetName(typeof(MMTypeOfTicket), MMTypeOfTicket.TicketFraud);
            var key = para.BuildCacheKey("TicketFraud", Region, UserID, ticketTypeName);

            try
            {
                fraudMMDetailResult = _memoryCacheService.GetValue<TicketDetailViewModel>(key);

                if (fraudMMDetailResult == null)
                {
                    para.TypeOfTickets = MMTypeOfTicket.TicketFraud;

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

        private IEnumerable<CustInfoModel> GetCustInfoByCustID(IEnumerable<long> custIds)
        {
            var custIdsToString = string.Join(",", custIds);
            if (!string.IsNullOrWhiteSpace(custIdsToString))
            {
                return _matchMonitorService.GetCustInfoByCustID(custIdsToString);
            }

            return Enumerable.Empty<CustInfoModel>();
        }

        private MMDetailTicketListViewModel GetTicketsFromMainDb(MMDetailInfoViewModel para, MMDetailFilterViewModel filter)
        {
            var verifiedTrans = new List<MatchMonitorDetailVerifyTransResultModel>();
            var ticketFromMainDb = new MMDetailTicketListViewModel();

            var transJson = _matchMonitorService.GetTransFilterSuspicousMGr(filter, null);

            var ticketDetailGroup = _matchMonitorService.GetTicketDetailAll(para, filter, transJson);

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
            ticketFromMainDb.LastSequenceID = para.LastScannedSequenceID;

            return ticketFromMainDb;
        }

        #endregion Detail Tickets

        #region Association Detect

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailManualDetectRpt)]
        public ActionResult DetectGroupAssWithOptions(DetectGroupAssFilterViewModel model)
        {
            var result = new DetectGroupAssociationListViewModel();

            var orgUserList = _associationDetectionService.ValidateUserName(model.RawUserNames);

            var data = _associationDetectionService.DetectAssociation(new DetectAssociationByUserNamesViewModel
            {
                UserNames = model.RawUserNames,
                IsBP = model.IsOptBP,
                IsDevice = model.IsOptDevice,
                IsIP = model.IsOptIP,
                IsAgent = model.IsOptAgent,
                IsIPLast3Days = model.IsOptIPLast3Days,
                Is3MatchesLast7Days = model.IsOpt3MatchesLast7Days
            }, orgUserList, false, false).Select(x => new DetectGroupAssociationItemViewModel(x)); ;

            result.ItemList = this.RearrangeAssGroup(data, true);

            var mGrList = new MGrListViewModel();
            mGrList.BuildMGrList(result.ItemList, model.RawCustIDs);

            return Json(new
            {
                data = result,
                mGrList = mGrList
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion Association Detect

        #region Verify Trans

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
                var logMessage = new LogMessage(ex.Message)
                 .AddMessage($"{Region} Verify Trans MM Detail Error: " + ex.Message + ex.StackTrace + "\n"
                  + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                  + ex.InnerException.StackTrace)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                result.ErrorCode = -1;
                result.ErrorMsg = ex.Message;
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion Verify Trans

        #region User Feedback
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailUserFeedback)]
        public ActionResult GetUserFeedback(UserFeedbackViewModel para)
        {
            var result = Enumerable.Empty<GetMMUserFeedbackViewModel>();

            var data = _matchMonitorService.GetUserFeedback(para);

            result = data.GroupBy(g => g.MMUserCommentID)
                .Select(g => new
                {
                    MMUserCommentID = g.Key,
                    IncorrectReasonTrans = g.Where(x => !string.IsNullOrEmpty(x.IncorrectReason)).OrderBy(x => x.IncorrectReason).Select(x =>
                      {
                          if (string.IsNullOrEmpty(x.FromTransInfo) && string.IsNullOrEmpty(x.ToTransInfo))
                          {
                              return "<span>" + x.IncorrectReason + "<br/></span>";
                          }
                          return "<span>" + x.IncorrectReason + " (" + x.FromTransInfo + " - " + x.ToTransInfo + ") <br/></span>";
                      }),
                    IncorrectReasonRaw = g.Where(x => !string.IsNullOrEmpty(x.IncorrectReason)).Select(x => new
                    {
                        IncorrectReason = x.IncorrectReason,
                        FromTransInfo = x.FromTransInfo,
                        ToTransInfo = x.ToTransInfo
                    }),
                    CorrectReason = g.Where(x => !string.IsNullOrEmpty(x.CorrectReason)).FirstOrDefault()?.CorrectReason ?? "Others",
                    Item = g.FirstOrDefault()
                }).Select(x => new GetMMUserFeedbackViewModel
                {
                    MMUserCommentID = x.MMUserCommentID,
                    UserComment = x.Item?.UserComment ?? "-",
                    IncorrectReason = x.IncorrectReasonTrans.Any() ? string.Join(" ", x.IncorrectReasonTrans) : "-",
                    IncorrectReasonRaw = JsonConvert.SerializeObject(x.IncorrectReasonRaw),
                    CorrectReason = x.CorrectReason,
                    FromTransInfo = x.Item?.FromTransInfo ?? string.Empty,
                    ToTransInfo = x.Item?.ToTransInfo ?? string.Empty,
                    LastModifiedBy = x.Item?.LastModifiedBy,
                    LastModifiedDate = x.Item?.LastModifiedDate.ToString(Consts.FormatDateTimeToString_yyyyMMdd)
                });

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailUserFeedbackInsert)]
        public ActionResult InsertUserFeedback(InsertUserFeedbackViewModel para)
        {
            para.UserID = CurrentSession.UserId;

            var result = _matchMonitorService.InsertUserFeedback(para);

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailUserFeedbackInsert)]
        public ActionResult UpdateUserFeedback(UpdateUserFeedbackViewModel para)
        {
            para.UserID = CurrentSession.UserId;

            var result = _matchMonitorService.UpdateUserFeedback(para);

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailUserFeedbackInsert)]
        public ActionResult DeleteUserFeedback(DeleteUserFeedbackViewModel para)
        {
            para.UserID = CurrentSession.UserId;

            var result = _matchMonitorService.DeleteUserFeedback(para);

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion User Feedback

        #endregion Detail Page

        #region Export to Excel
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailExportToExcelRpt)]
        public ActionResult ExportToExcelMMDetail(FormCollection from)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var matchInfo = serializer.Deserialize<ExExlMatchInfoViewModel>(from["match"]);
            var isDisplaySingleGrid = from["isDisplaySingleGrid"];

            if (!string.IsNullOrEmpty(isDisplaySingleGrid) && isDisplaySingleGrid.ToLower() == "true")
            {
                var items = serializer.Deserialize<List<ExExcelTicketInfoViewModel>>(from["items"]);

                var templatePath = Server.MapPath(MMDetailSingleGridTemplate);

                var fileName = string.Format("MM_{0}_{1}_{2}.xlsx", matchInfo.MatchName, matchInfo.BetTypeName, matchInfo.EventDateStr);

                var excelFile = _matchMonitorService.ExportExcelMMDetailSG(items, fileName, templatePath);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
            else
            {
                var homeMItems = serializer.Deserialize<List<ExExcelTicketInfoViewModel>>(from["homes"]);
                var awayMItems = serializer.Deserialize<List<ExExcelTicketInfoViewModel>>(from["aways"]);
                var templatePath = Server.MapPath(MMDetailDoubleGridTemplate);

                var fileName = string.Format("MM_{0}_{1}_{2}.xlsx", matchInfo.MatchName, matchInfo.BetTypeName, matchInfo.EventDateStr);

                var excelFile = _matchMonitorService.ExportExcelMMDetailDG(homeMItems, awayMItems, fileName, templatePath);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
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
                nTicket.ReasonIDList = string.Empty;
                nTicket.TypeOfTicket = MMTypeOfTicket.TicketAll;

                ticketResult.Add(nTicket);
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