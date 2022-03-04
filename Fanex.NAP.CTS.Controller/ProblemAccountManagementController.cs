namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProblemAccountManagement, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProblemAccountManagementController : BaseController
    {
        private readonly IProblemAccountManagementService _problemAccountManagementService;
        private readonly ICommonService _commonService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ProblemAccountManagementController()
        {
            _commonService = new CommonService();
            _problemAccountManagementService = new ProblemAccountManagementService();
        }

        public ActionResult Index()
        {
            var subscriber = _commonService.GetSubscriberGroups().ToList();

            return View(subscriber);
        }

        public ActionResult ValidateUsernames(string userNames)
        {
            var data = "";

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetProblemAccount(string userNames)
        {
            var data = new List<ProblemAccountViewModel>()
            {
                new ProblemAccountViewModel
                {
                     UserName = "A",
                     Site = "Bet88",
                     Currency = "USD",
                     Category = "Group Betting",
                     Comment = "Test",
                     CreatedDateStr = "Feb 28, 2022"
                }
            };

            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
