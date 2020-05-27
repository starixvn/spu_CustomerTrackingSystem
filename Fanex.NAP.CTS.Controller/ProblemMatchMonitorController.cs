namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.ProblemMatchMonitor;
    using System;
    using System.Linq;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.ProblemMatchMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProblemMatchMonitorController : BaseController
    {
        private readonly IProblemMatchMonitorService _problemMatchMonitorService;

        public ProblemMatchMonitorController() 
            => _problemMatchMonitorService = new ProblemMatchMonitorService();

        public ActionResult Index()
        {
            var moveBetBalanceUpDate = _problemMatchMonitorService.GetMoveBetBalanceUpDate();            

            return View(moveBetBalanceUpDate);
        }

        [HttpGet]
        public ActionResult GetSports()
        {
            var viewModel = Enumerable.Empty<SportViewModel>();

            var model = _problemMatchMonitorService.GetSports();

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

            var model = _problemMatchMonitorService.GetLeagues(eventDate, sportId);

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
        public ActionResult GetProblemMatchMonitor(DateTime eventDate, string leagueIds, string matchStatus)
        {
            var result = Enumerable.Empty<ProblemMatchMonitorViewModel>();

            var model = _problemMatchMonitorService.GetProblemMatchMonitor(eventDate, leagueIds, matchStatus);

            if (model != null || model.MatchInfos != null)
            {
                result = model.MatchInfos.Select(x => new ProblemMatchMonitorViewModel
                {
                    MatchId = x.MatchId,
                    MatchCode = x.MatchCode,
                    LeagueName = x.LeagueName,
                    HomeName = CachedRef.GetTeamName(x.HomeId),
                    AwayName = CachedRef.GetTeamName(x.AwayId),
                    MatchScore = x.LiveHomeScore + " - " + x.LiveAwayScore,
                    Status = x.EventStatus,
                    GroupBettingCount = _problemMatchMonitorService.CountTicketFraud(model.SuspiciousTicketInfos, x.MatchId, 13),
                    HedgingCount = _problemMatchMonitorService.CountTicketFraud(model.SuspiciousTicketInfos, x.MatchId, 14)
                });
            }

            return Json(new
            {                
                recordsTotal = result.Count(),
                recordsFiltered = result.Count(),
                data = result
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
                            FSFunctionKeys.ProblemMatchMonitor, 
                            NAP.Core.Business.Constant.Policy.Host)
            });
        }

        [HttpGet]
        public ActionResult GetMatchInfo(int matchId, int fraudCaseId)
        {
            var viewModel = new DetailMatchInfoViewModel();
            var model = _problemMatchMonitorService.GetDetailMatchInfo(matchId, fraudCaseId);

            if (model != null)
            {
                viewModel = new DetailMatchInfoViewModel
                {                   
                    EventDate = model.EventDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
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
            var model = _problemMatchMonitorService.GetDetailTickets(matchId, fraudCaseId);

            if (model != null && model.Any())
            {
                result.SiteList = model.GroupBy(x => new { x.SiteId, x.SiteName })
                              .Select(x => new DetailSite { SiteId = x.Key.SiteId, SiteName = x.Key.SiteName });

                result.BetTypeList = model.GroupBy(x => new { x.BettypeId, x.BettypeName })
                              .Select(x => new DetailBetType { BetTypeId = x.Key.BettypeId, BetTypeName = x.Key.BettypeName });

                result.TicketList = model.Select(x => new DetailTicket
                {
                    BetId = x.TransId,
                    BetDate = x.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    BettypeId = x.BettypeId,
                    Username = x.Username,
                    CurrencyName = x.CurrencyName,
                    SiteId = x.SiteId,
                    SiteName = x.SiteName,
                    Choice = x.BetTeam,
                    LiveBetScore = string.Format("{0}-{1}", x.LiveHomeScore, x.LiveAwayScore),
                    HtmlChoice = _problemMatchMonitorService.GetHtmlChoice(x),
                    Stake = x.Stake,
                    Odds = x.Odds,
                    TicketStatus = x.TicketStatus,
                    IsVoid = false,
                    HighlightStatus = x.HighlightStatus
                });
            }

            return Json(new
            {
                recordsTotal = result.TicketList.Count(),
                recordsFiltered = result.TicketList.Count(),
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

    }
}