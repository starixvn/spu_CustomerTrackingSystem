namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using System.Web.Mvc;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Implement;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using Fanex.NAP.CTS.ViewModel.ProblemCaseMonitor;
    using System.Linq;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProblemCaseMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProblemCaseMonitorController : BaseController
    {
        private readonly IProblemCaseMonitorService _problemCaseMonitorService;
        private readonly ICommonService _commonService;

        public ProblemCaseMonitorController()
        {
            _problemCaseMonitorService = new ProblemCaseMonitorService();
            _commonService = new CommonService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ProblemCaseMonitor,
                            NAP.Core.Business.Constant.Policy.Host);

            var userId = CurrentSession.UserId;
            var problemCaseOptions = _problemCaseMonitorService.GetProblemCasesFilter((int)STATIC_LIST.PROBLEMCASE, true).ToList();
            var subscriber = _commonService.GetSubscriberGroups().ToList();

            return View(new ProblemCaseViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = userId,
                ProblemCases = problemCaseOptions,
                Subscriber = subscriber
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
