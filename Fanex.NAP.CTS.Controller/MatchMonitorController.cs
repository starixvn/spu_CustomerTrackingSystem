namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Common.Extensions;
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
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.DetectGroup;
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
        private const string MMDetailDoubleGridTemplate = "~/App_Data/Excel Template/MatchMonitor/DoubleGridTemplate.xlsx";
        private const string MMDetailSingleGridTemplate = "~/App_Data/Excel Template/MatchMonitor/SingleGridTemplate.xlsx";
        private readonly IMatchMonitorService _matchMonitorService;
        private readonly IAssociationDetectionService _associationDetectionService;
        private readonly ICommonService _commonService;
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly string MMDetailRegion = "MMDetail";     

        public override string BaseControllerName { get => Consts.MenuItemMonitor; }

        public MatchMonitorController()
        {
            _matchMonitorService = new MatchMonitorService();
            _commonService = new CommonService();
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
                var logMessage = new LogMessage(ex.Message)
                    .AddMessage("Get Match Monitor Error: " + ex.Message + ex.StackTrace + "\n"
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

        [HttpPost]
        public ActionResult GetMMFilterBetTypeBySportID(string sportIDs)
        {
            var result = this.GetFilterBetTypeBySportID(sportIDs);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<MatchMonitorBetTypeViewModel> GetFilterBetTypeBySportID(string sportIDs)
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

            return result;
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
                   .AddMessage("Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
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
                    if (para.TypeOfTickets == MatchMonitorQueryType.TicketFraud)
                    {
                        result = GetFraudMMFilter(para);
                    }
                    else if(para.IsSupportShowAllTickets)
                    {
                        result = GetAllMMFilter(para);
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddMessage("Get Match Monitor Filter Error: " + ex.Message + ex.StackTrace + "\n"
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

            var removeKeyFraud = $"{Enum.GetName(typeof(MatchMonitorQueryType), MatchMonitorQueryType.TicketFraud)}_{para.EventDateToString}_{para.MatchID}_{para.BetTypeID}_{para.BetID}_{para.LiveIndicator}";
            _memoryCacheService.RemoveKeyContainsValue(removeKeyFraud);

            var removeKeyAll = $"{Enum.GetName(typeof(MatchMonitorQueryType), MatchMonitorQueryType.TicketAll)}_{para.EventDateToString}_{para.MatchID}_{para.BetTypeID}_{para.BetID}_{para.LiveIndicator}";
            _memoryCacheService.RemoveKeyContainsValue(removeKeyAll);

            switch (para.TypeOfTickets)
            {
                case MatchMonitorQueryType.TicketFraud:
                    {
                        result = GetFraudMMFilter(para);

                        if (para.IsSupportShowAllTickets)
                        {
                            Task.Run(() =>
                            {
                                para.TypeOfTickets = MatchMonitorQueryType.TicketAll;

                                _ = GetAllMMFilter(para);
                            });
                        }

                        break;
                    }
                case MatchMonitorQueryType.TicketAll:
                    {
                        para.TypeOfTickets = MatchMonitorQueryType.TicketFraud;

                        GetFraudMMDetailForCache(para);

                        if (para.IsSupportShowAllTickets)
                        {                          
                            para.TypeOfTickets = MatchMonitorQueryType.TicketAll;

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
            para.TypeOfTickets = MatchMonitorQueryType.TicketFraud;
            var ticketTypeName = Enum.GetName(typeof(MatchMonitorQueryType), para.TypeOfTickets);

            var key = para.BuildCacheKey(nameof(GetFraudMMFilter), MMDetailRegion, ticketTypeName);

            lock (key)
            {
                fraudFilterResult = _memoryCacheService.GetValue<MMDetailFilterViewModel>(key);
                
                if (fraudFilterResult == null)
                {
                    var fraudTicket = GetFraudMMDetailForCache(para);
                    
                    if (fraudTicket != null)
                    {                       
                        fraudFilterResult = _matchMonitorService.GetFraudFilterInfoDetail(para, fraudTicket);

                        _memoryCacheService.PushValueAsync(key, fraudFilterResult);
                    }
                }
            }

            fraudFilterResult = fraudFilterResult ?? new MMDetailFilterViewModel();
            fraudFilterResult.IsDisplaySingleGrid = _matchMonitorService.IsDisplaySingleGrid(para.BetTypeID);

            return fraudFilterResult;
        }

        private MMDetailFilterViewModel GetAllMMFilter(MMDetailInfoViewModel para)
        {
            var allFilterResult = new MMDetailFilterViewModel();
            var ticketTypeName = Enum.GetName(typeof(MatchMonitorQueryType), para.TypeOfTickets);
            var key = para.BuildCacheKey(nameof(GetAllMMFilter), MMDetailRegion, ticketTypeName);
            
            try
            {
                allFilterResult = _memoryCacheService.GetValue<MMDetailFilterViewModel>(key);

                if (allFilterResult == null)
                {
                    var fraudTicket = GetFraudMMDetailForCache(para);

                    if (fraudTicket != null)
                    {
                        allFilterResult = _matchMonitorService.GetFraudFilterInfoDetail(para, fraudTicket);

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
                    .AddMessage("Get All MM Filter Error: " + ex.Message + ex.StackTrace + "\n"
                     + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                     + ex.InnerException.StackTrace)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                throw;
            }

            allFilterResult = allFilterResult ?? new MMDetailFilterViewModel();         
            allFilterResult.IsDisplaySingleGrid = _matchMonitorService.IsDisplaySingleGrid(para.BetTypeID);

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
                   .AddMessage("Get Match Monitor Total Ticket Error: " + ex.Message + ex.StackTrace + "\n"
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
                    case MatchMonitorQueryType.TicketFraud:
                        {
                            var fraudTicket = GetFraudMMDetailForCache(para);

                            if (fraudTicket != null)
                            {
                                mdResult = GetFraudMMDetail(para, filter, fraudTicket);
                            }
                            
                            break;
                        }
                    case MatchMonitorQueryType.TicketAll when para.IsSupportShowAllTickets:
                        {
                            mdResult = GetAllMMDetail(para, filter);

                            if (mdResult != null)
                            {
                                var ctsCustIDHomeIsZero = mdResult.HomeMatchItemList.Where(x => x.CTSCustID == 0).Select(x => x);
                                var ctsCustIDAwayIsZero = mdResult.AwayMatchItemList.Where(x => x.CTSCustID == 0).Select(x => x);

                                if (ctsCustIDHomeIsZero != null && ctsCustIDHomeIsZero.Any())
                                {
                                    LogService.Error($"Get Match Monitor Detail with CTSCustIDHome=0: { JsonConvert.SerializeObject(ctsCustIDHomeIsZero)}");
                                }

                                if (ctsCustIDAwayIsZero != null && ctsCustIDAwayIsZero.Any())
                                {
                                    LogService.Error($"Get Match Monitor Detail with CTSCustIDAway=0: { JsonConvert.SerializeObject(ctsCustIDAwayIsZero)}");
                                }
                            }

                            break;
                        }
                }
            }
            catch (Exception ex)
            {              
                var logMessage = new LogMessage(ex.Message)
                   .AddMessage("Get Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
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
            para.TypeOfTickets = MatchMonitorQueryType.TicketAll;
            var mdResult = new MMDetailTicketListViewModel();
            var ticketDetailGroup = new TicketDetailViewModel();

            try
            {
                if (!_matchMonitorService.IsDisplaySingleGrid(para.BetTypeID))
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

                        mdResult.HomeMatchItemList = mdItemList?.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.H), StringComparison.OrdinalIgnoreCase)
                               || string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.O), StringComparison.OrdinalIgnoreCase))
                              .OrderByDescending(x => x.TransTime).ThenByDescending(x=>x.TransId).ToList();
                        mdResult.AwayMatchItemList = mdItemList?.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.A), StringComparison.OrdinalIgnoreCase)
                                || string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.U), StringComparison.OrdinalIgnoreCase))
                               .OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();

                        mdResult.RecordsTotal = mdItemList?.Count() ?? 0;                        
                    }
                    else
                    {
                        mdResult = GetTicketsFromMainDb(para, filter);
                    }
                }
                else if (para.TypeOfTickets == MatchMonitorQueryType.TicketAll)
                {
                    mdResult = GetTicketsFromMainDb(para, filter);
                }               

                return mdResult;
            }
            catch (Exception ex)
            {              
                var logMessage = new LogMessage(ex.Message)
                 .AddMessage("Get All Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
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

                mmStatic = _matchMonitorService.GetMMStaticList();

                var custIDs = matchDetailInfo.Custs.Select(x=>x.CustID).Distinct();
                var custInfo = GetCustInfoByCustID(custIDs);
               
                var ticketMergeGroups = fraudTicket.AGroupReasonMerged;

                var transJson = fraudTicket.GetTransByFilterIncludeSuspicous(filter, ticketMergeGroups);

                var ticketDetailGroup = _matchMonitorService.GetTicketDetailByFraudTranIDs(para, filter, transJson);

                custIDs = ticketDetailGroup.TicketDetails.Select(ticket => ticket.CustId).Distinct();

                custInfo = GetCustInfoByCustID(custIDs);
              
                var mergeInfo = _matchMonitorService.DrawColorFraudMMDetailWithMergeReasonAGroup(matchDetailInfo.VerifiedTrans, custInfo, ticketDetailGroup.TicketDetails, mergeGroup, mmStatic.Reasons);

                var mdItemList = MappingRobotStatus(mergeInfo, para.BetTypeID);

                if (_matchMonitorService.IsDisplaySingleGrid(para.BetTypeID))
                {
                    mdResult.ItemList = mdItemList.OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                }
                else
                {
                    mdResult.HomeMatchItemList = mdItemList.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.H), StringComparison.OrdinalIgnoreCase)
                            || string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.O), StringComparison.OrdinalIgnoreCase))
                           .OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                    mdResult.AwayMatchItemList = mdItemList.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.A), StringComparison.OrdinalIgnoreCase)
                            || string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.U), StringComparison.OrdinalIgnoreCase))
                           .OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                }

                mdResult.RecordsTotal = mdItemList?.Count() ?? 0;
            }

            return mdResult;
        }

        private TicketDetailViewModel GetFraudMMDetailForCache(MMDetailInfoViewModel filter)
        {
            var para = filter.Clone();
            var fraudMMDetailResult = new TicketDetailViewModel();
            var ticketTypeName = Enum.GetName(typeof(MatchMonitorQueryType), MatchMonitorQueryType.TicketFraud);
            var key = para.BuildCacheKey("TicketFraud", MMDetailRegion, ticketTypeName);

            try
            {
                fraudMMDetailResult = _memoryCacheService.GetValue<TicketDetailViewModel>(key);

                if (fraudMMDetailResult == null)
                {
                    var matchDetailInfo = _matchMonitorService.GetMatchMonitorDetail(para);

                    if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
                    {
                        // para.TypeOfTickets = MatchMonitorQueryType.TicketFraud;

                        fraudMMDetailResult = _matchMonitorService.GetTicketDetailFraud(matchDetailInfo, para);

                        fraudMMDetailResult.MatchDetailInfo = matchDetailInfo;

                        fraudMMDetailResult.AGroupReasonMerged = _matchMonitorService.MergeFraudReasonAGroup(fraudMMDetailResult);

                        // write log for trace issue
                        if (para.MatchID == 73893434)
                        {
                            LogService.Info($"Trace Issue - Get MM Detail AGroupReasonMerged: {JsonConvert.SerializeObject(fraudMMDetailResult.AGroupReasonMerged)}");                           
                        }
                        // write log for trace issue end

                        _memoryCacheService.PushValue(key, fraudMMDetailResult);
                    }
                }
            }
            catch (Exception ex)
            {                
                var logMessage = new LogMessage(ex.Message)
                  .AddMessage("Get Fraud MM Detail For Cache Error: " + ex.Message + ex.StackTrace + "\n"
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

            if (_matchMonitorService.IsDisplaySingleGrid(para.BetTypeID))
            {
                ticketFromMainDb.ItemList = mdItemList.OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();               
            }
            else
            {
                ticketFromMainDb.HomeMatchItemList = mdItemList?.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.H), StringComparison.OrdinalIgnoreCase)
                              || string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.O), StringComparison.OrdinalIgnoreCase))
                             .OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
                ticketFromMainDb.AwayMatchItemList = mdItemList?.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.A), StringComparison.OrdinalIgnoreCase)
                        || string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.U), StringComparison.OrdinalIgnoreCase))
                       .OrderByDescending(x => x.TransTime).ThenByDescending(x => x.TransId).ToList();
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
            var userNames = model.RawUserNames.Split(',');
            var userNameList = string.Join(",", userNames.Select(x => x.ToUpper()).Distinct());

            var orgUserList = userNames.Select(x => new ValidateUserNameModel { UserName = x });

            var validatedList = _commonService.ValidateUsernames(userNameList, out bool isException);

            orgUserList = orgUserList.Join(validatedList, c1 => c1.UserName.ToLower(), c2 => c2.UserName.ToLower(),
                       (c1, c2) => new ValidateUserNameModel
                       {
                           UserName = c2.UserName,
                           CustID = c2.CustID,
                           CTSCustID = c2.CTSCustID,
                           IsInternal = c2.IsInternal
                       });
            var custIgnoreInternals = orgUserList.Where(x => !x.IsInternal);
            var custIDIgnoreInternals = string.Join(",", custIgnoreInternals.Select(x => x.CustID));
            var ctsCustIDIgnoreInternals = string.Join(",", custIgnoreInternals.Select(x => x.CTSCustID));
            var custInfo = _matchMonitorService.GetCustInfoByCustID(custIDIgnoreInternals);
            var mergeGroupInfo = new MergeGroupInfoModel { CTSCustIDs = ctsCustIDIgnoreInternals };

            Parallel.Invoke(() =>
            {                
                var detectAssByIP = model.IsOptIPLast3Days ? _associationDetectionService.DetectAssociationByIP(custIDIgnoreInternals) : Enumerable.Empty<DetectAssociationBySharedIPModel>();

                mergeGroupInfo.AssociationByIPJson = this.GetAssociationCustByIP(detectAssByIP, custInfo);
            }, () =>
            {
                var detectAssByAssMatches = model.IsOpt3MatchesLast7Days ? _associationDetectionService.DetectAssociationBySharedMatches(custIDIgnoreInternals) : Enumerable.Empty<DetectAssociationBySharedIPMatchesModel>();

                mergeGroupInfo.AssociationByMatchesJson = this.GetAssociationCustByMatch(detectAssByAssMatches, custInfo);
            },
            () =>
            {
                if (model.IsOptBP || model.IsOptDevice || model.IsOptIP)
                {
                    var assByPrimary = _associationDetectionService.DetectAssociationByPrimary(new DetectAssociationByPrimaryFilterModel
                    {
                        CTSCustIDs = ctsCustIDIgnoreInternals,
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
                    assByAgent = _associationDetectionService.DetectAssociationByAgent(ctsCustIDIgnoreInternals);

                mergeGroupInfo.AssociationByAgentJson = JsonConvert.SerializeObject(assByAgent);
            });

            var data = _associationDetectionService.DetectGroupDetectAssociation(mergeGroupInfo).Select(x => new DetectGroupAssociationItemViewModel(x));

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
                 .AddMessage("Verify Trans MM Detail Error: " + ex.Message + ex.StackTrace + "\n"
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

            if (!string.IsNullOrEmpty(isDisplaySingleGrid)  && isDisplaySingleGrid.ToLower() == "true")
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
                nTicket.TypeOfTicket = MatchMonitorQueryType.TicketAll;

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