namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.ManageSubscriber;
    using Fanex.NAP.Core.Web.Attribute;
    using System.Web.Mvc;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProblemCaseMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProblemCaseMonitorController : BaseController
    {
        //private readonly IManageSubscriberService _manageSubscriberService;

        public ProblemCaseMonitorController()
        {
           // _manageSubscriberService = new ManageSubscriberService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ManageSubscriber,
                            NAP.Core.Business.Constant.Policy.Host);

            var userId = CurrentSession.UserId;

            return View(new ManageSubscriberViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = userId
            });
        }

        [HttpPost]
        public ActionResult GetProblemCasesReport()
        {
            //var subscribers = _manageSubscriberService.GetSubscribers().OrderByDescending(c => c.IsActive).ThenByDescending(c => c.CreatedDate).ThenByDescending(c=>c.TerminatedDate);

            return Json(new
            {
               data = string.Empty
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
