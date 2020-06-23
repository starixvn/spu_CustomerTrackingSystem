namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Common;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

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
        public ActionResult GetProblemAccountMonitor(DateTime eventDate, string leagueIds, string matchStatus, string siteIds)
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

        [HttpGet]
        public ActionResult Detail(int matchId, int fraudCaseId)
        {
            IUserService userService = new UserService();
            ISecurityService securityService = new SecurityService();
            var userId = CurrentSession.UserId.ToString();
            var onUserId = CurrentSession.OnUserId.ToString();
            var time = securityService.GetTime();

            var voidRequestToTVSParam = new VoidRequestToTVSParam
            {
                UserId = Cryptography.CfsEnCrypt(userId),
                OnUserId = Cryptography.CfsEnCrypt(onUserId),
                Time = Cryptography.CfsEnCrypt(time),
                SubIssueTypeId = 27,
                ActionId = 5,
                TransIds = "456789",
                TargetUrl = "http://ha.nexdev.net:7307/L1/TicketVoidingSystem/AddRequest/FromOtherSites"
            };

            return View(new DetailViewModel
            {
                MatchId = matchId,
                FraudCaseId = fraudCaseId,
                VoidRequestToTVSParam = voidRequestToTVSParam,
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
        public ActionResult GetTickets(int matchId, int fraudCaseId, string stakeFrom, string stakeTo)
        {
            var result = new DetailTicketViewModel();
            var total = 0;
            var tasks = new List<Task>();
            var ticketVoidingsModel = Enumerable.Empty<DetailTicketVoidingInRequestModel>();
            var ticketsModel = _probleAccountMonitorService.GetDetailTickets(matchId, fraudCaseId, stakeFrom, stakeTo);

            if (ticketsModel != null && ticketsModel.Any())
            {
                tasks.Add(Task.Run(() =>
                {
                    result.SiteList = ticketsModel.GroupBy(x => new { x.SiteId, x.SiteName })
                        .Select(x => new DetailSite { SiteId = x.Key.SiteId, SiteName = x.Key.SiteName });
                }));

                tasks.Add(Task.Run(() =>
                {
                    result.BetTypeList = ticketsModel.GroupBy(x => new { x.BettypeId, x.BettypeName })
                        .Select(x => new DetailBetType { BetTypeId = x.Key.BettypeId, BetTypeName = x.Key.BettypeName });
                }));

                tasks.Add(Task.Run(() =>
                {
                    result.CurrencyList = ticketsModel.GroupBy(x => new { x.CurrencyId, x.CurrencyName })
                        .Select(x => new DetailCurrency { CurrencyId = x.Key.CurrencyId, CurrencyName = x.Key.CurrencyName });
                                
                }));

                tasks.Add(Task.Run(() =>
                {
                    var tranIdList = string.Join(",", ticketsModel.Select(x => x.TransId));

                    ticketVoidingsModel = _probleAccountMonitorService.GetTicketVoidingInRequest(matchId, tranIdList);
                }));
          
                Task.WhenAll(tasks.ToArray()).Wait();
                
                total = ticketsModel.Count();

                result.TicketList = ticketsModel.Select(x => new DetailTicket
                {
                    BetId = x.TransId,
                    BetDate = x.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    BettypeId = x.BettypeId,
                    BettypeName = x.BettypeName,
                    Username = x.Username,
                    CurrencyId = x.CurrencyId,
                    CurrencyName = x.CurrencyName,
                    SiteId = x.SiteId,
                    SiteName = x.SiteName,
                    Choice = x.BetTeam,
                    LiveBetScore = string.Format("{0} - {1}", x.LiveHomeScore, x.LiveAwayScore),
                    HtmlChoice = _probleAccountMonitorService.GetHtmlChoice(x),
                    Stake = x.Stake,
                    Odds = x.Odds,
                    TicketStatus = x.TicketStatus,
                    TicketStatusId = x.TicketStatusId,
                    VoidRequestId = x.TicketStatusId != 102 // Void
                                    ? _probleAccountMonitorService.GetVoidRequestId(ticketVoidingsModel, x.TransId)
                                    : string.Empty,
                    HighlightStatus = x.HighlightStatus
                }).OrderByDescending(x=>x.BetDate);

                result.TotalHighlightStatusGreen = result.TicketList.Count(x => x.HighlightStatus.Equals(0) && x.VoidRequestId == string.Empty);
            }

            return Json(new
            {
                recordsTotal = total,
                recordsFiltered = total,
                data = result
            }, JsonRequestBehavior.AllowGet);
        }       
    }
}