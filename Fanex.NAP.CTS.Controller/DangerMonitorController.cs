namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System.Linq;
    using System.Web.Mvc;

    public class DangerMonitorController : BaseController
    {
        private readonly IDangerMonitorService _dangerMonitorService;
        private readonly MatchMonitorController _matchMonitor;
        public DangerMonitorController() {
            _dangerMonitorService = new DangerMonitorService();
            _matchMonitor = new MatchMonitorController();
        }

        #region danger main
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetSportBettype()
        {
            var sportBettypeList = _dangerMonitorService.GetSportBettypes();

            return Json(new {
                data = sportBettypeList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDangerMonitor(DangerMonitorSearchModel filter)
        {
            var data = _dangerMonitorService.GetDangerMonitor(filter);
            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region danger detail
        public ActionResult Detail(MatchMonitorDetailViewModel para)
        {
            para.IsSupportShowAllTickets = true;
            return _matchMonitor.Detail(para);
            //return View();
        }
        #endregion
    }
}