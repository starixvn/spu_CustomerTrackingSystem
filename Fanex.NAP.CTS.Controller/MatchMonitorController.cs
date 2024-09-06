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
    using Fanex.NAP.CTS.Core.Utilities;
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
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.MarkPA;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.Parlay;

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

        #region SportType BetType
        [HttpPost]
        public ActionResult GetSportBettype()
        {
            var sportBettypeList = GetSportBettypes();

            return Json(new
            {
                data = sportBettypeList
            }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<MatchMonitorSporTypeBetTypeViewModel> GetSportBettypes()
        {
            var betTypes = GetBetTypeSettings(null, true, false);

            var filter = new SportBettypeModel
            {
                ProductId = (int)ProductType.SportBook,
                BettypeList = string.Join(",", betTypes.Select(x => x.BetTypeID).Distinct())
            };

            var mainBetTypes = _commonService.GetMainSportBettype(filter);

            var mainBetTypeDictionary = mainBetTypes.ToDictionary(x => $"{x.SporttypeId}_{x.LeagueGroupID}_{x.BettypeId}", y => y);

            return betTypes.Select(x =>
            {
                var item = mainBetTypeDictionary.GetValueOrDefault($"{x.SportTypeID}_{x.LeagueGroupID}_{x.BetTypeID}");

                return new MatchMonitorSporTypeBetTypeViewModel
                {
                    SportGroup = item.SportGroup,
                    SportNameDisplay = item.SportNameDisplay,
                    LeagueGroupID = item.LeagueGroupID,
                    SportType = item.SporttypeId,
                    SportName = item.SportName,
                    BetTypeID = x.BetTypeID,
                    BetTypeName = x.BetTypeNameDisplay,
                    BetID = _commonService.GetBetID(x.BetIDPattern, x.NameDetailsX),
                    BetChoiceType = x.BetChoiceType,
                    BetChoiceHome = x.BetChoiceHome,
                    BetChoiceAway = x.BetChoiceAway,
                    PriorityOrder = x.PriorityOrder,
                    BetIDPattern = x.BetIDPattern,
                    NameDetailsX = x.NameDetailsX
                };
            });
        }

        private IEnumerable<MatchMonitorBetTypeViewModel> GetBetTypeSettingsWithSupportSuspicious(string sportIDs, bool isFilterMMDetection, bool isFilterMMParlayDetection)
        {
            var bettypeSetting = GetBetTypeSettings(sportIDs, isFilterMMDetection, isFilterMMParlayDetection);

            return bettypeSetting.Select(x => new MatchMonitorBetTypeViewModel
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

        private IEnumerable<BetTypeSettingModel> GetBetTypeSettings(string sportIDs, bool isFilterMMDetection, bool isFilterMMParlayDetection)
        {
            var paras = new BetTypeSettingParaModel
            {
                FunctionID = (int)BetTypeSettingsFunctionID.MMBetType,
                BettypeIDList = null,
                SportTypeIDList = sportIDs,
                IsFilterMMDetection = isFilterMMDetection,
                IsFilterMMParlayDetection = isFilterMMParlayDetection,
                IsShowDetailsX = true,
                Status = 1
            };

            return _commonService.GetBetTypeSettings(paras);
        }
        #endregion

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
           
            var reasonGroups = GetReasonGroups(false);

            return View(new MatchMonitorViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = CurrentSession.UserId,
                ReasonGroups = reasonGroups,
                Reasons = reasons,
            });
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorRpt, "isSkipTracking")]
        public ActionResult GetMatchMonitor(MatchMonitorSearchViewModel para, bool? isSkipTracking)
        {
            var matchMonitors = Enumerable.Empty<GetMatchMonitorViewModel>();

            try
            {
                if (para != null && para.IsValid())
                {
                    #region Comment for write test

                    var matchFromCTSInfos = Enumerable.Empty<GetMatchMonitorModel>();

                    if (para.DateType == 1)
                        matchFromCTSInfos = GetMMByScanDate(para);
                    else
                        matchFromCTSInfos = GetMMByEventDate(para);

                    var dicSumTickets = GetMMFraudTicketsSummary(matchFromCTSInfos, TicketType.Single);

                    var dicSumParlayTickets = GetMMFraudTicketsSummary(matchFromCTSInfos, TicketType.Parlay);

                    matchMonitors = MergeMatchMonitorWithMMStaticAndTicketsSummary(matchFromCTSInfos, dicSumTickets, dicSumParlayTickets);

                   // matchMonitors = GenerateMMParlay_Test();

                    #endregion Comment for write test
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
       
        private IEnumerable<ReasonGroupStaticListViewModel> GetReasonGroups(bool isInitGroupNone)
        {
            var reasonGroupItems = _commonService.GetStaticLists((int)StaticListID.Reason, true);
            var result = new List<ReasonGroupStaticListViewModel>();

            if (isInitGroupNone)
            {
                result.Add(new ReasonGroupStaticListViewModel
                {
                    ReasonGroupName = Consts.ReasonDetectNone,
                    ReasonItems = new List<ReasonStaticListViewModel>() { new ReasonStaticListViewModel { ReasonID = "-1", ReasonName = "-", IsSelected = true } }
                });
            }

            result.Add(new ReasonGroupStaticListViewModel
            {
                ReasonGroupName = ReasonDetectBySystem,
                ReasonItems = reasonGroupItems.Where(x => x.ListNameDisplay == ReasonDetectBySystem)
                    .Select(x => new ReasonStaticListViewModel { ReasonID = x.ItemValue, ReasonName = x.ItemNameDisplay, IsSelected = x.ItemDefaultValue == "1" })
                    .OrderBy(x => x.ReasonName)
            });

            result.Add(new ReasonGroupStaticListViewModel
            {
                ReasonGroupName = ReasonDetectBySystemParlay_Display,
                ReasonItems = reasonGroupItems.Where(x => x.ListNameDisplay == ReasonDetectBySystemParlay)
                    .Select(x => new ReasonStaticListViewModel { ReasonID = x.ItemValue, ReasonName = x.ItemNameDisplay, IsSelected = x.ItemDefaultValue == "1" })
                    .OrderBy(x => x.ReasonName)
            });

            result.Add(new ReasonGroupStaticListViewModel
            {
                ReasonGroupName = ReasonDetectByAI,
                ReasonItems = reasonGroupItems.Where(x => x.ListNameDisplay == ReasonDetectByAI)
                    .Select(x => new ReasonStaticListViewModel { ReasonID = x.ItemValue, ReasonName = x.ItemNameDisplay, IsSelected = false })
                    .OrderBy(x => x.ReasonName)
            });

            return result;
        }

        private IEnumerable<GetMatchMonitorViewModel> MergeMatchMonitorWithMMStaticAndTicketsSummary(IEnumerable<GetMatchMonitorModel> matchFromCTSInfos,
            Dictionary<string, MMFraudTicketsSummaryViewModel> dicSumTickets,
            Dictionary<string, MMFraudTicketsSummaryViewModel> dicSumParlayTickets)
        {
            var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);
            var bettypeSettings = GetBetTypeSettingsWithSupportSuspicious(null, true, false);

            return
                matchFromCTSInfos?
                    .Select(x =>
                        {
                            var result = new GetMatchMonitorViewModel(x, reasons);
                            var betChoiceHome = bettypeSettings.FirstOrDefault(y => y.BetTypeID == result.BettypeID)?.BetChoiceHome;
                            var betChoiceAway = bettypeSettings.FirstOrDefault(y => y.BetTypeID == result.BettypeID)?.BetChoiceAway;
                            if (x.TicketType == (int)TicketType.Single)
                            {
                                if (dicSumTickets != null)
                                {
                                    SetValueMMTicketsSummaryFromDictionary(result, betChoiceHome, dicSumTickets, PropertyNameOfSummaryHomeList);

                                    SetValueMMTicketsSummaryFromDictionary(result, betChoiceAway, dicSumTickets, PropertyNameOfSummaryAwayList);
                                }
                            }
                            else
                            {
                                if (dicSumParlayTickets != null)
                                {
                                    SetValueMMTicketsSummaryFromDictionary(result, betChoiceHome, dicSumParlayTickets, PropertyNameOfSummaryHomeList);

                                    SetValueMMTicketsSummaryFromDictionary(result, betChoiceAway, dicSumParlayTickets, PropertyNameOfSummaryAwayList);
                                }
                            }  
                            
                            return result;
                        })
                    .OrderBy(c => c.MatchSection);
        }

        private void SetValueMMTicketsSummaryFromDictionary(GetMatchMonitorViewModel result, string betChoice,
            Dictionary<string, MMFraudTicketsSummaryViewModel> dicSumTickets, string[] propertyNameOfSummaryList)
        {
            if (dicSumTickets.TryGetValue(result.BuildTicketsSummaryDictionaryKey(betChoice), out var mmFraudTicketsSummary))
            {
                result.SetPropertyValue(typeof(GetMatchMonitorViewModel).GetProperty(propertyNameOfSummaryList[0]), mmFraudTicketsSummary.Turnover);

                result.SetPropertyValue(typeof(GetMatchMonitorViewModel).GetProperty(propertyNameOfSummaryList[1]), mmFraudTicketsSummary.BetCount);
            }
        }

        private Dictionary<string, MMFraudTicketsSummaryViewModel> GetMMFraudTicketsSummary(IEnumerable<GetMatchMonitorModel> matchFromCTSInfos, TicketType ticketType)
        {          
            if (ticketType == TicketType.Single)
            {
                var mmFraudTicketsSummaryModel = matchFromCTSInfos.Where(x => x.TicketType == (int)TicketType.Single)
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
            else
            {
                var mmFraudTicketsSummaryModel = matchFromCTSInfos.Where(x => x.TicketType == (int)TicketType.Parlay)
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

                return _matchMonitorService.GetMMParlayFraudTicketsSummary(mmFraudTicketsSummaryModel)?.ToDictionary(x => x.BuildDictionaryKey());
            }
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
                    x.HomeName = y.HomeName;
                    x.AwayName = y.AwayName;
                    x.LeagueName = y.LeagueName;
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
                        SportTypeId = m1.SportTypeId,
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
        #endregion

        #region Detail Page
        [HttpGet]
        public ActionResult Detail(MatchMonitorDetailViewModel para)
        {
            IUserService userService = new UserService();

            try
            {
                ViewBag.GroupCCList = _commonService.GetGroupCustomerClass();
                ViewBag.ReasonGroups = this.GetReasonGroups(true);
                ViewBag.EventDateToString = para?.EventDateToString;
                ViewBag.MatchID = para?.MatchID;
                ViewBag.BetTypeID = para?.BetTypeID;
                ViewBag.BettypeName = para?.BettypeName ?? string.Empty;
                ViewBag.BetIDToString = para?.BetID.ToString();
                ViewBag.BetTypeSettings = GetBetTypeSettingsWithSupportSuspicious(para.SportType.ToString(), false, false);
                ViewBag.MapX = para?.MX;
                ViewBag.MapYY = para?.MY;
                ViewBag.HomeName = para?.HomeName.ToUpper() ?? string.Empty;
                ViewBag.AwayName = para?.AwayName.ToUpper() ?? string.Empty;
                ViewBag.MatchName = ViewBag.HomeName + "_" + ViewBag.AwayName;
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
                ViewBag.MatchSection = para?.MatchSection ?? 0;
                ViewBag.IsVisibleHoldKey = para?.MatchSection != (int)MATCHMONITOR_SECTION.Closed;
                ViewBag.HoldKeyStatus = GetHoldKeyStatus(para?.MatchID ?? 0, para?.MatchSection ?? 0);
                ViewBag.TicketType = (int)para.TicketType;
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
                #region Comment for Test
                if (para.IsImportNewTicket)
                {
                    result = GetMatchMonitorFilterImportNew(para);
                }
                else
                {
                    if (para.TypeOfTickets == MMViewModeFromUI.TicketFraud)
                    {
                        result = GetFraudMMFilter(para);
                    }
                    else if (para.IsSupportShowAllTickets)
                    {
                        result = GetAllMMFilter(para);
                    }
                }

                //result = GetMatchMonitorFilter_Test();

                #endregion Comment for Test
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

            var removeKeyFraud = para.BuildCacheKey(string.Empty, Region, UserID, Enum.GetName(typeof(MMViewModeFromUI), MMViewModeFromUI.TicketFraud));
            _memoryCacheService.RemoveKeyContainsValue(removeKeyFraud);

            var removeKeyAll = para.BuildCacheKey(string.Empty, Region, UserID, Enum.GetName(typeof(MMViewModeFromUI), MMViewModeFromUI.TicketAll));
            _memoryCacheService.RemoveKeyContainsValue(removeKeyAll);

            // log for check remove cache
            LogMMClearCacheNotClean(para, removeKeyFraud);
            LogMMClearCacheNotClean(para, removeKeyAll);

            switch (para.TypeOfTickets)
            {
                case MMViewModeFromUI.TicketFraud:
                    {
                        result = GetFraudMMFilter(para);

                        if (para.IsSupportShowAllTickets)
                        {
                            Task.Run(() =>
                            {
                                para.TypeOfTickets = MMViewModeFromUI.TicketAll;

                                _ = GetAllMMFilter(para);
                            });
                        }

                        break;
                    }
                case MMViewModeFromUI.TicketAll:
                    {
                        para.TypeOfTickets = MMViewModeFromUI.TicketFraud;

                        GetFraudMMDetailForCache(para);

                        if (para.IsSupportShowAllTickets)
                        {
                            para.TypeOfTickets = MMViewModeFromUI.TicketAll;

                            result = GetAllMMFilter(para);
                        }

                        break;
                    }
            }

            return result;
        }

        private void LogMMClearCacheNotClean(MMDetailInfoViewModel para, string key)
        {
            var keyContainsValue = _memoryCacheService.GetKeyContainsValue(key);
            if (keyContainsValue.Any())
            {
                var logMessage = new LogMessage($"{Region} Match Monitor Clear Cache Not Clean")
                   .AddMessage("Params " + JsonConvert.SerializeObject(para) + "\n")
                   .AddMessage("Keys " + JsonConvert.SerializeObject(keyContainsValue) + "\n")
                   .ToString();

                LogService.Error(logMessage);
            }
        }

        [HttpGet]
        public ActionResult GetDataLogMM(string key)
        {
            return Json(new
            {
                data = _memoryCacheService.GetValue<object>(key)
            }, JsonRequestBehavior.AllowGet);
        }

        private MMDetailFilterViewModel GetFraudMMFilter(MMDetailInfoViewModel para)
        {
            var fraudFilterResult = new MMDetailFilterViewModel();
            para.TypeOfTickets = MMViewModeFromUI.TicketFraud;
            var viewModeName = Enum.GetName(typeof(MMViewModeFromUI), para.TypeOfTickets);

            var key = para.BuildCacheKey(nameof(GetFraudMMFilter), Region, UserID, viewModeName);

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

                        _memoryCacheService.PushValue(key, fraudFilterResult);
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
            var viewModeName = Enum.GetName(typeof(MMViewModeFromUI), para.TypeOfTickets);
            var key = para.BuildCacheKey(nameof(GetAllMMFilter), Region, UserID, viewModeName);

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

                        _memoryCacheService.PushValue(key, allFilterResult);
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

        #region Generate Data Test
        private IEnumerable<GetMatchMonitorViewModel> GenerateMMParlay_Test()
        {
            var matchMonitors = new List<GetMatchMonitorViewModel>();

            var mmModel = new GetMatchMonitorModel()
            {
                BetID = 0,
                BettypeID = 1,
                AwayID = 11,
                AwayName = "Anh",
                HomeID = 22,
                HomeName = "TBN",
                ID = 1,
                BettypeNameDisplay = "U/O",
                IsVerified = false,
                LeagueID = 1,
                LeagueName = "EURO 2024",
                MatchID = 100,
                MatchSection = 1,
                SportTypeId = 1,
                TicketType = (int)TicketType.Parlay,
                EventDate = DateTime.Now,
                Reason = "5"
            };

            var reasons = _commonService.GetStaticLists((int)StaticListID.Reason, true);

            var mmViewModel = new GetMatchMonitorViewModel(mmModel, reasons)
            {
                HomeTotalAmount = 1000,
                HomeBetCount = 10,
                AwayTotalAmount = 2000,
                AwayBetCount = 20
            };

            matchMonitors.Add(mmViewModel);

            return matchMonitors;
        }

        private MMDetailFilterViewModel GetMatchMonitorFilter_Test()
        {
            var result = new MMDetailFilterViewModel()
            {
                TicketStatusList = new List<int>() { 1, 2 },
                HDPList = new List<decimal>() { 1, 2 },
                DropdownListFilter = new MMDetailFilterDropdownListViewModel()
                {
                    TicketStatusList = new List<TicketStatusViewModel>() { new TicketStatusViewModel { TicketStatus = 112, TicketStatusName = "Won" } }
                }
            };

            return result;
        }

        private int GetMatchMonitorTotalTicket_Test()
        {
            return 1;
        }

        private MMDetailTicketListViewModel GetMatchMonitorDetail_Test()
        {
            var mdResult = new MMDetailTicketListViewModel();

            var parlayTickets = new List<ParlayTicketInfoViewModel>()
            {
                new ParlayTicketInfoViewModel
                {
                    HomeName = "VN", AwayName = "TL",
                    BetTeam = "Home",
                    BetTypeName = "Handicap",
                    SportName = "Giao Huu",
                    LeagueName = "GH",
                    GlobalShowTime = "8/16/2024 01:00 AM"
                }
            };

            mdResult.HomeMatchItemList = new List<MatchMonitorDetailItemViewModel>()
            {
                new MatchMonitorDetailItemViewModel
                {
                    BetID = "0",
                    BetTypeID = 1,
                    BetTeam = "h",
                    CustomerClass = 301,
                    CurrencyId = 1,
                    CurrencyName = "HK",
                    Amount = 1000,
                    TransId = 45678912223456789,
                    TransTime = DateTime.Now,
                    TransTimeToString = DateTime.Now.ToString(Consts.FormatDateTimeToString_ddMMyyhhmmss),
                    Username = "User Test 1",
                    SubParlayCount = 2
                    //ParlayTicket = parlayTickets
                },
                new MatchMonitorDetailItemViewModel
                {
                    BetID = "0",
                    BetTypeID = 3,
                    BetTeam = "h",
                    CustomerClass = 301,
                    CurrencyId = 1,
                    CurrencyName = "HK",
                    Amount = 1500,
                    TransId = 45678912223456777,
                    TransTime = DateTime.Now,
                    TransTimeToString = DateTime.Now.ToString(Consts.FormatDateTimeToString_ddMMyyhhmmss),
                    Username = "User Test 2",
                    SubParlayCount = 2
                    //ParlayTicket = parlayTickets
                }
            };

            return mdResult;
        }

        private IEnumerable<ParlayTicketInfoViewModel> GetParlayTicketByRefNo()
        {
            return new List<ParlayTicketInfoViewModel>() {
                new ParlayTicketInfoViewModel{
                    HomeName = "VN",
                    AwayName = "TL",
                    BetTeam = "Home",
                    BetTypeName = "Handicap",
                    SportName = "Giao Huu",
                    LeagueName = "GH",
                    GlobalShowTime = "8/16/2024 01:00 AM"
                }
            };             
        }

        #endregion Generate Data Test

        #endregion Detail Filter

        #region Detail Total Tickets
        [HttpPost]
        public ActionResult GetMatchMonitorTotalTicket(MMDetailInfoViewModel info, MMDetailFilterViewModel filter)
        {
            long recordsTotal = 0;

            try
            {
                #region Comment for test

                //var fraudTicket = GetFraudMMDetailForCache(info);

                //var transJson = _matchMonitorService.GetTransFilterSuspicousMGr(filter, fraudTicket);

                //recordsTotal = _matchMonitorService.GetTotalTicketDetail(info, filter, 0, transJson);

                recordsTotal = GetMatchMonitorTotalTicket_Test();

                #endregion Comment for test
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
                #region Comment for test

                //switch (para.TypeOfTickets)
                //{
                //    case MMViewModeFromUI.TicketFraud:
                //        {
                //            var fraudTicket = GetFraudMMDetailForCache(para);

                //            if (fraudTicket != null)
                //            {
                //                mdResult = GetFraudMMDetail(para, filter, fraudTicket);
                //            }

                //            break;
                //        }
                //    case MMViewModeFromUI.TicketAll when para.IsSupportShowAllTickets:
                //        {
                //            var internalCurrencyIDs = new List<int> { 20, 27, 28 };

                //            mdResult = GetAllMMDetail(para, filter);

                //            if (mdResult != null)
                //            {
                //                var ctsCustIDHomeIsZero = mdResult.HomeMatchItemList.Where(x => x.CTSCustID == 0 && !internalCurrencyIDs.Any(c => c == x.CurrencyId)).Select(x => x);
                //                var ctsCustIDAwayIsZero = mdResult.AwayMatchItemList.Where(x => x.CTSCustID == 0 && !internalCurrencyIDs.Any(c => c == x.CurrencyId)).Select(x => x);

                //                if (ctsCustIDHomeIsZero != null && ctsCustIDHomeIsZero.Any())
                //                {
                //                    LogService.Error($"{Region} Get Match Monitor Detail with CTSCustIDHome=0: {JsonConvert.SerializeObject(ctsCustIDHomeIsZero)}");
                //                }

                //                if (ctsCustIDAwayIsZero != null && ctsCustIDAwayIsZero.Any())
                //                {
                //                    LogService.Error($"{Region} Get Match Monitor Detail with CTSCustIDAway=0: {JsonConvert.SerializeObject(ctsCustIDAwayIsZero)}");
                //                }
                //            }

                //            break;
                //        }
                //}

                mdResult = GetMatchMonitorDetail_Test();

                #endregion Comment for test
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
            para.TypeOfTickets = MMViewModeFromUI.TicketAll;
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
                else if (para.TypeOfTickets == MMViewModeFromUI.TicketAll)
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
            var viewModeName = Enum.GetName(typeof(MMViewModeFromUI), MMViewModeFromUI.TicketFraud);
            var key = filter.TicketType == TicketType.Single
                ? para.BuildCacheKey("TicketFraud", Region, UserID, viewModeName)
                : para.BuildCacheKey("TicketFraudParlay", Region, UserID, viewModeName);

            try
            {
                fraudMMDetailResult = _memoryCacheService.GetValue<TicketDetailViewModel>(key);

                if (fraudMMDetailResult == null)
                {
                    para.TypeOfTickets = MMViewModeFromUI.TicketFraud;

                    var matchDetailInfo = filter.TicketType == TicketType.Single
                        ? _matchMonitorService.GetMatchMonitorDetail(para)
                        : _matchMonitorService.GetMatchMonitorParlayDetail(para);

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

        #region Parlay Detail
        [HttpPost]
        public ActionResult GetMatchMonitorParlayFilter(MMDetailInfoViewModel para)
        {
            para.TicketType = TicketType.Parlay;

            var result = GetMatchMonitorFilter(para);

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetParlayTicketByRefNo(long refNo)
        {
            return Json(new
            {
                data = GetParlayTicketByRefNo()
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

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
            }, orgUserList, false).Select(x => new DetectGroupAssociationItemViewModel(x)); ;

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

            if (data.Any())
            {
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
            }

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

        #region Hold-Key
        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailHoldKeyUpdate)]
        public ActionResult UpdateHoldKeyStatus(HoldKeyUpdateViewModel paramter)
        {
            var result = new HoldKeyResultViewModel(
                            new HoldKeyStatusGetViewModel
                            {
                                IsHold = paramter.IsCurrentStatus,
                                UserName = paramter.UserName
                            });
            try
            {
                if (paramter.MatchId != 0
                    && paramter.MatchSection != (int)MATCHMONITOR_SECTION.Closed
                    && !string.IsNullOrEmpty(ConfigSettings.UpdateHoldKeyMMDetailURL))
                {
                    paramter.UserId = CurrentSession.UserId;
                    paramter.UserName = CurrentSession.Username;

                    _matchMonitorService.LogHoldKeyAction(CurrentSession.UserId, paramter.IsHold, paramter.MatchId);

                    var updateResult = _matchMonitorService.UpdateHoldKeyStatus(paramter, ConfigSettings.UpdateHoldKeyMMDetailURL);

                    if (!updateResult.isSuccess)
                    {
                        LogService.Error($"{ConfigSettings.EnvironmentName} - Match Monitor Detail - Update hold key status - Exception: "
                                         + updateResult.msg
                                         + $" DataInput: {JsonConvert.SerializeObject(paramter) + "\n"}");
                    }

                    var statusResult = GetHoldKeyStatus(paramter.MatchId, paramter.MatchSection);

                    result.HoldKeyStatus = statusResult;
                    result.IsSuccess = updateResult.isSuccess;
                    result.Message = updateResult.msg;
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{ConfigSettings.EnvironmentName} - Match Monitor Detail - Update hold key status - Exception: "
                                 + ex.Message
                                 + ex.StackTrace + "\n"
                                 + ex?.InnerException?.Message + "\n"
                                 + $"DataInput: {JsonConvert.SerializeObject(paramter) + "\n"}"
                                 + ex?.InnerException?.Source
                                 + ex?.InnerException?.StackTrace);
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        private HoldKeyStatusGetViewModel GetHoldKeyStatus(Int64 matchId, int matchStatus)
        {
            var result = new HoldKeyStatusGetViewModel();

            try
            {
                if (matchId != 0
                    && matchStatus != (int)MATCHMONITOR_SECTION.Closed
                    && !string.IsNullOrEmpty(ConfigSettings.GetHoldKeyMMDetailURL))
                {
                    var statusResult = _matchMonitorService.GetHoldKeyStatus(matchId, ConfigSettings.GetHoldKeyMMDetailURL);

                    if (statusResult != null && !statusResult.isSuccess)
                    {
                        throw new Exception($"GetHoldKeyStatus - call Adminsite's api unsuccessfully (IP = {matchId})");
                    }

                    if (statusResult != null
                        && statusResult.holdKeyInfo != null
                        && statusResult.holdKeyInfo.Any())
                    {
                        var matchInfo = statusResult.holdKeyInfo.FirstOrDefault();

                        if (matchInfo.matchId == matchId)
                        {
                            result = new HoldKeyStatusGetViewModel
                            {
                                IsHold = matchInfo.isHold,
                                UserName = matchInfo.userName
                            };
                        }
                        else
                        {
                            throw new Exception($"GetHoldKeyStatus - Adminsite's api returns incorrect matchId (IP = {matchId} || OP = {matchInfo.matchId})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{ConfigSettings.EnvironmentName} - Match Monitor Detail - Get hold key status - Exception: "
                  + ex.Message
                  + ex.StackTrace + "\n"
                  + ex?.InnerException?.Message + "\n"
                  + $"DataInput: {JsonConvert.SerializeObject(matchId) + "\n"}"
                  + ex?.InnerException?.Source
                  + ex?.InnerException?.StackTrace);
            }

            return result;
        }

        #endregion

        #region Ticket Voiding
        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailTicketVoiding)]
        public ActionResult RedirectToTicketVoiding(string tranIds)
        {
            var result = new TicketVoidingViewModel();
            bool isValid = false;
            const int maxTranIds = 2000;

            try
            {
                if (!string.IsNullOrEmpty(tranIds))
                {

                    List<string> tranList = tranIds.Split(',').ToList();

                    if (tranList.Count() <= maxTranIds)
                    {
                        result.OnUserId = Common.Cryptography.CfsEnCrypt(CurrentSession.OnUserId.ToString());
                        result.UserId = Common.Cryptography.CfsEnCrypt(CurrentSession.UserId.ToString());
                        result.Time = Common.Cryptography.CfsEnCrypt(DateTime.Now.ToString());
                        result.TransIds = tranIds;
                        isValid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{ConfigSettings.EnvironmentName} - Match Monitor Detail - Redirect to Ticket Voiding - Exception: "
                                 + ex.Message
                                 + ex.StackTrace + "\n"
                                 + ex?.InnerException?.Message + "\n"
                                 + $"DataInput: {JsonConvert.SerializeObject(tranIds) + "\n"}"
                                 + ex?.InnerException?.Source
                                 + ex?.InnerException?.StackTrace);
            }

            return Json(new
            {
                data = result,
                isValid = isValid
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Mark PA
        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailMarkPA)]
        public ActionResult MarkPA(string usernames)
        {
            var result = new MMDetailMarkPAViewModel();
            var isValid = false;

            try
            {
                if (!string.IsNullOrEmpty(usernames))
                {
                    var userList = new HashSet<string>(usernames.Split(','));

                    if (userList.Count > 0 && userList.Count <= 1001)
                    {
                        result.Usernames = usernames;
                        result.MarkPARedirectURL = ConfigSettings.MarkPAURL;
                        isValid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{ConfigSettings.EnvironmentName} - Match Monitor Detail - Redirect to Mark PA - Exception: "
                                 + ex.Message
                                 + ex.StackTrace + "\n"
                                 + ex?.InnerException?.Message + "\n"
                                 + $"DataInput: {usernames + "\n"}"
                                 + ex?.InnerException?.Source
                                 + ex?.InnerException?.StackTrace);
            }

            return Json(new
            {
                data = result,
                isValid = isValid
            }, JsonRequestBehavior.AllowGet);
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
                nTicket.TypeOfTicket = MMViewModeFromUI.TicketAll;

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