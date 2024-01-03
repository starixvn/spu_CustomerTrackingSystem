namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using System.Web.Mvc;

    public class DangerMonitorController : BaseController
    {
        private readonly IDangerMonitorService _dangerMonitorService;
        public DangerMonitorController() {
            _dangerMonitorService = new DangerMonitorService();
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
            return Json(new
            {
                data = string.Empty
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region danger detail
        public ActionResult Detail()
        {
            return View();
        }
        #endregion
    }
}