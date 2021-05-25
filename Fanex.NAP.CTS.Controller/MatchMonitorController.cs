namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using System.Web.Mvc;

    //[NapSuperAuth]
    //[NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : BaseController
    {
        public MatchMonitorController()
        {

        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            //var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
            //                NapSettings.Settings.SiteId,
            //                FSFunctionKeys.ManageSubscriber,
            //                NAP.Core.Business.Constant.Policy.Host);

            //var userId = CurrentSession.UserId;

            return View();
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