namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Common;
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProblemAccountMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProblemAccountMonitorController : BaseController
    {
        private readonly IProblemAccountMonitorService _probleAccountMonitorService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ProblemAccountMonitorController() 
            => _probleAccountMonitorService = new ProblemAccountMonitorService();

        public ActionResult Index()
        {
            var moveBetBalanceUpDate = _probleAccountMonitorService.GetMoveBetBalanceUpDate();            

            return View(moveBetBalanceUpDate);
        }

        [HttpGet]
        public ActionResult GetSports()
        {
            var viewModel = Enumerable.Empty<SportViewModel>();

            var model = _probleAccountMonitorService.GetSports();

            if (model != null)
            {
                viewModel = model.Select(x => new SportViewModel
                {
                    SportType = x.SportType,
                    SportName = x.SportName
                });
            }

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetLeagues(DateTime eventDate, string sportIds)
        {
            var viewModel = Enumerable.Empty<LeagueViewModel>();

            var model = _probleAccountMonitorService.GetLeagues(eventDate, sportIds);

            if (model != null)
            {
                viewModel = model.Select(x => new LeagueViewModel
                {
                    LeagueId = x.leagueid,
                    LeagueName = x.leaguename
                });
            }

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllSubscriberGroups()
        {
            var viewModel = Enumerable.Empty<SubscriberGroupViewModel>();

            var model = _probleAccountMonitorService.GetAllSubscriberGroups();

            if (model != null)
            {
                viewModel = model.Where(x => x.IsActive)
                    .GroupBy(g => new { g.SubscriberGroupID, g.SubscriberGroupName })
                    .Select(x => new { Group = x.Key, SiteList = x.ToList() })
                    .Select(x => new SubscriberGroupViewModel
                    {
                        SubscriberGroupID = x.Group.SubscriberGroupID,
                        SubscriberGroupName = x.Group.SubscriberGroupName,
                        SiteList = x.SiteList.Select(y => new SiteViewModel
                        {
                            SiteID = y.SiteID,
                            SiteName = y.SiteName,
                            DisplayOrder = y.DisplayOrder
                        })
                    });
            }

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.ProblemAccountMonitor, "isSkipTracking")]
        public ActionResult GetProblemAccountMonitor(DateTime eventDate, string leagueIds, string matchStatus, string siteIds, bool? isSkipTracking)
        {
            var viewModel = Enumerable.Empty<ProblemAccountMonitorViewModel>();
            var total = 0;
            var model = _probleAccountMonitorService.GetProblemAccountMonitor(eventDate, leagueIds, matchStatus, siteIds);

            if (model != null || model.MatchInfos != null)
            {
                total = model.MatchInfos.Count();
                viewModel = model.MatchInfos.Select(x => new ProblemAccountMonitorViewModel
                {
                    MatchId = x.MatchId,
                    MatchCode = x.MatchCode,
                    LeagueName = x.LeagueName,
                    SportName = x.SportName,
                    HomeName = CachedRef.GetTeamName(x.HomeId),
                    AwayName = CachedRef.GetTeamName(x.AwayId),
                    MatchScore = x.LiveHomeScore + " - " + x.LiveAwayScore,
                    Status = x.EventStatus,
                    GroupBettingCount = _probleAccountMonitorService.CountTicketFraud(model.SuspiciousTicketInfos, x.MatchId, 13),
                    HedgingCount = _probleAccountMonitorService.CountTicketFraud(model.SuspiciousTicketInfos, x.MatchId, 14)
                }).OrderByDescending(x => x.GroupBettingCount + x.HedgingCount);
            }

            return Json(new {
                recordsTotal = total,
                recordsFiltered = total,
                data = viewModel
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Detail(int matchId, string matchStatus, int fraudCaseId, string siteIds)
        {
            IUserService userService = new UserService();
            
            return View(new DetailViewModel
            {
                MatchId = matchId,
                MatchStatus = matchStatus.ToLower(),
                FraudCaseId = fraudCaseId,
                SiteIds = siteIds,
                IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ProblemAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Host)
            });
        }

        [HttpGet]
        public ActionResult GetMatchInfo(int matchId, int fraudCaseId)
        {
            var viewModel = new DetailMatchInfoViewModel();
            var model = _probleAccountMonitorService.GetDetailMatchInfo(matchId, fraudCaseId);

            if (model != null)
            {
                viewModel = new DetailMatchInfoViewModel
                {                   
                    EventDate = model.EventDate != DateTime.MinValue ? model.EventDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss) : "",
                    MatchName = string.Format("{0} vs {1}", CachedRef.GetTeamName(model.HomeId), CachedRef.GetTeamName(model.AwayId)),
                    Score = string.Format("{0} - {1} / {2} - {3}", model.HTHomeScore, model.HTAwayScore, model.finalhomescore, model.finalawayscore),
                    Evidence = model.FraudCaseName
                };                
            }

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTickets(int matchId, int fraudCaseId, string stakeFrom, string stakeTo, string siteIds)
        {
            var result = new DetailTicketViewModel();
            var total = 0;        
            var ticketsModel = _probleAccountMonitorService.GetDetailTickets(matchId, fraudCaseId, stakeFrom, stakeTo, siteIds);                       

            if (ticketsModel != null && ticketsModel.Any())
            {
                total = ticketsModel.Count();

                result = GetResultTicketData(ticketsModel, matchId);
            }

            return Json(new
            {
                recordsTotal = total,
                recordsFiltered = total,
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetGetInfoVoidRequestToTVS()
        {
            ISecurityService securityService = new SecurityService();
            var userId = CurrentSession.UserId.ToString();
            var onUserId = CurrentSession.OnUserId.ToString();
            var time = securityService.GetTime();

            var voidRequestToTVSParam = new VoidRequestToTVSParam
            {
                EnCryptedUserId = Cryptography.CfsEnCrypt(userId),
                EnCryptedOnUserId = Cryptography.CfsEnCrypt(onUserId),
                EnCryptedTime = Cryptography.CfsEnCrypt(time),
                SubIssueTypeId = ConfigSettings.VoidRequestToTVSSubIssueTypeId,
                ActionId = ConfigSettings.VoidRequestToTVSActionId,
                RedirectUrl = ConfigSettings.VoidRequestToTVSURL
            };

            return Json(voidRequestToTVSParam, JsonRequestBehavior.AllowGet);
        }

        private DetailTicketViewModel GetResultTicketData(IEnumerable<DetailTicketsModel> ticketsModel, int matchId)
        {
            var result = new DetailTicketViewModel();
            var ticketVoidingsModels = Enumerable.Empty<DetailTicketVoidingInRequestModel>();
            var associatedGroupModels = Enumerable.Empty<DetailAssociatedGroupModel>();
            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    var custGroups = _probleAccountMonitorService.GetCustomerGroup(ticketsModel.ToList());
                    GetGroupForDeTailTicketModel(ticketsModel, custGroups);

                    result.GroupList = ticketsModel.GroupBy(x => new { x.GroupID })
                         .Select(s => new DetailGroup { GroupId = s.Key.GroupID, GroupName = s.Key.GroupID.ToString() }).Distinct();
                }),

                Task.Run(() =>
                {
                    try
                    {
                        var custIds = ticketsModel.Select(x => x.CustID).Distinct();
                        associatedGroupModels = _probleAccountMonitorService.GetAssociatedGroup(custIds, ConfigSettings.GetAssociatedGroup);
                    }
                    catch (Exception ex)
                    {
                         LogService.Error(ex.Message, ex);
                    }
                }),

                Task.Run(() =>
                {
                    result.SiteList = ticketsModel.GroupBy(x => new { x.SiteId, x.SiteName })
                        .Select(x => new DetailSite { SiteId = x.Key.SiteId, SiteName = x.Key.SiteName });
                }),

                Task.Run(() =>
                {
                    result.BetTypeList = ticketsModel.GroupBy(x => new { x.BettypeId, x.BettypeName })
                        .Select(x => new DetailBetType { BetTypeId = x.Key.BettypeId, BetTypeName = x.Key.BettypeName });
                }),

                Task.Run(() =>
                {
                    result.CurrencyList = ticketsModel.GroupBy(x => new { x.CurrencyId, x.CurrencyName })
                        .Select(x => new DetailCurrency { CurrencyId = x.Key.CurrencyId, CurrencyName = x.Key.CurrencyName });
                }),

                Task.Run(() =>
                {
                    result.TicketStatusList = ticketsModel.GroupBy(x => new { x.TicketStatusId, x.TicketStatus })
                        .Select(x => new DetailTicketStatus { TicketStatusId = x.Key.TicketStatusId, TicketStatusName = x.Key.TicketStatus });
                }),

                Task.Run(() =>
                {
                    var tranIdList = string.Join(",", ticketsModel.Select(x => x.TransID));

                    ticketVoidingsModels = _probleAccountMonitorService.GetTicketVoidingInRequest(matchId, tranIdList);
                })
            };
          
            Task.WhenAll(tasks.ToArray()).Wait();

            result.TicketList = ticketsModel
                                    .Select(x => SelectDetailTicket(x, ticketVoidingsModels, associatedGroupModels))
                                    .OrderByDescending(x=>x.BetDate);

            result.AssociatedGroupList = result.TicketList.GroupBy(x => x.AssociatedGroupName)              
                .Where(x => x.Key != string.Empty)
                .Select(x => x.Key)
                .OrderBy(x=> x);

            result.TotalHighlightStatusWhite = result.TicketList.Count(x => x.HighlightStatus.Equals(0) 
                && x.VoidRequestId == string.Empty
                && x.TicketStatusId.Equals(0));

            return result;
        }

        private DetailTicket SelectDetailTicket(DetailTicketsModel ticketModel
            , IEnumerable<DetailTicketVoidingInRequestModel> ticketVoidingsModels
            , IEnumerable<DetailAssociatedGroupModel> associatedGroupModels)
        {
            var requestId = string.Empty;
            // Void
            if (ticketModel.TicketStatusId != 102)
            {
                var voidTicket = ticketVoidingsModels.FirstOrDefault(x => x.transid.Equals(ticketModel.TransID));

                if (voidTicket != null)
                    requestId = voidTicket.RequestId.ToString();
            }           

            var associatedGroupName = string.Empty;
            var associatedGroup = associatedGroupModels.FirstOrDefault(x => x.CustId.Equals(ticketModel.CustID));
            if (associatedGroup != null)
                associatedGroupName = associatedGroup.GroupId.ToString();

            return new DetailTicket
            {
                BetId = ticketModel.TransID,
                BetDate = ticketModel.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                BettypeId = ticketModel.BettypeId,
                BettypeName = ticketModel.BettypeName,
                Username = ticketModel.Username,
                CurrencyId = ticketModel.CurrencyId,
                CurrencyName = ticketModel.CurrencyName,
                SiteId = ticketModel.SiteId,
                SiteName = ticketModel.SiteName,
                Choice = ticketModel.BetTeam,
                LiveBetScore = string.Format("{0} - {1}", ticketModel.LiveHomeScore, ticketModel.LiveAwayScore),
                HtmlChoice = _probleAccountMonitorService.GetHtmlChoice(ticketModel),
                Stake = ticketModel.Stake,
                Odds = ticketModel.Odds,
                TicketStatus = ticketModel.TicketStatus,
                TicketStatusId = ticketModel.TicketStatusId,
                VoidRequestId = requestId,
                HighlightStatus = ticketModel.HighlightStatus,
                AssociatedGroupName = associatedGroupName,
                GroupId = ticketModel.GroupID,
                GroupName = ticketModel.GroupID.ToString()
            };
        }

        private void GetGroupForDeTailTicketModel(IEnumerable<DetailTicketsModel> ticketsModel, IEnumerable<DetailCustomerGroupsModel> customerGroupsModel)
        {
            ticketsModel.ToList().ForEach(ticket =>
            {
                var customerGroup = customerGroupsModel.ToList().FirstOrDefault(custGroup => ticket.CustID == custGroup.CustID && ticket.TransID == custGroup.TransID);

                if (customerGroup != null)
                {
                    ticket.GroupID = customerGroup.GroupID;
                }

            });
        }
    }
}