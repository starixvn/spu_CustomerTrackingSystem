namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.BaseClasses;
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
                    MatchScore = x.LiveHomeScore + "-" + x.LiveAwayScore,
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

        [HttpPost]
        public ActionResult Detail(int matchId, int fraudCaseId)
        {


            return new JsonResult();
        }
    }

}