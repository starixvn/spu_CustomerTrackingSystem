namespace Fanex.NAP.CTS.Controller
{
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
        public ActionResult GetLeagues(DateTime eventDate, int sportId)
        {
            var viewModel = Enumerable.Empty<LeagueViewModel>();

            var model = _probleAccountMonitorService.GetLeagues(eventDate, sportId);

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

        [HttpPost]
        public ActionResult GetProblemAccountMonitor(DateTime eventDate, string leagueIds, string matchStatus)
        {
            var viewModel = Enumerable.Empty<ProblemAccountMonitorViewModel>();
            var total = 0;
            var model = _probleAccountMonitorService.GetProblemAccountMonitor(eventDate, leagueIds, matchStatus);

            if (model != null || model.MatchInfos != null)
            {
                total = model.MatchInfos.Count();
                viewModel = model.MatchInfos.Select(x => new ProblemAccountMonitorViewModel
                {
                    MatchId = x.MatchId,
                    MatchCode = x.MatchCode,
                    LeagueName = x.LeagueName,
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

            return View(new DetailViewModel
            {
                MatchId = matchId,
                FraudCaseId = fraudCaseId,
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
        public ActionResult GetTickets(int matchId, int fraudCaseId)
        {
            var result = new DetailTicketViewModel();
            var total = 0;
            var tasks = new List<Task>();
            var ticketVoidingsModel = Enumerable.Empty<DetailTicketVoidingInRequestModel>();
            var ticketsModel = _probleAccountMonitorService.GetDetailTickets(matchId, fraudCaseId);

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
                    Username = x.Username,
                    CurrencyName = x.CurrencyName,
                    SiteId = x.SiteId,
                    SiteName = x.SiteName,
                    Choice = x.BetTeam,
                    LiveBetScore = string.Format("{0} - {1}", x.LiveHomeScore, x.LiveAwayScore),
                    HtmlChoice = _probleAccountMonitorService.GetHtmlChoice(x),
                    Stake = x.Stake,
                    Odds = x.Odds,
                    TicketStatus = x.TicketStatus,
                    VoidRequestId = _probleAccountMonitorService.GetVoidRequestId(ticketVoidingsModel, x.TransId),
                    HighlightStatus = x.HighlightStatus
                }).OrderByDescending(x=>x.BetDate);
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