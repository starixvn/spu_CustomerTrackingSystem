namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.CTS.Model.ManageSubscriber;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    //[NapSuperAuth]
    //[NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : BaseController
    {
        private readonly IMatchMonitorService _matchMonitorService;

        public MatchMonitorController()
        {
            _matchMonitorService = new MatchMonitorService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.MatchMonitor,
                            NAP.Core.Business.Constant.Policy.Host);

            var userId = CurrentSession.UserId;

            return View(new MatchMonitorViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = userId
            });
        }

        [HttpPost]
        public ActionResult GetMatchMonitor(MatchMonitorFilterViewModel matchMonitorFilterViewModel)
        {
            var matchMonitors = new List<MatchMonitorModel>();
                
            try
            {
                matchMonitors = _matchMonitorService.GetMatchMonitor(matchMonitorFilterViewModel);
            }
            catch(Exception e)
            {
                LogService.Error("Match Monitor Error", e); 
            }

            return Json(new
            {
                data = matchMonitors
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail()
        {
            IUserService userService = new UserService();

            //var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
            //                NapSettings.Settings.SiteId,
            //                FSFunctionKeys.ManageSubscriber,
            //                NAP.Core.Business.Constant.Policy.Host);

            //var userId = CurrentSession.UserId;

            return View("Detail");
        }
    }
}