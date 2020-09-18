namespace Fanex.NAP.CTS.Controller
{
    using System.Web.Mvc;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Implement;
    using System.Collections.Generic;
    using Fanex.NAP.CTS.ViewModel.ProbationManagement;
    using System;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProbationManagement, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProbationManagementController : BaseController
    {
        private readonly IProbationManagementService _probationService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ProbationManagementController()
        {
            _probationService = new ProbationManagementService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ProbationManagement,
                            NAP.Core.Business.Constant.Policy.Host);

            return View(isHostPermission);
        }

        [HttpPost]
        public ActionResult GetProbationCategory(string username, int dayLeft, decimal minMargin, decimal maxMargin)
        {
            var data = _probationService.GetProbationCategory(username, dayLeft, minMargin, maxMargin);
            var viewModel = GenerateDataTest();

            return Json(new
            {
                data = viewModel
            }, JsonRequestBehavior.AllowGet);
        }

        private List<ProbationCategoryViewModel> GenerateDataTest()
        {
            var list = new List<ProbationCategoryViewModel>();
            list.Add(new ProbationCategoryViewModel
            {
                CTSCustId = 1,
                CustId = 1,
                UserName = "username1",
                SiteName = "12Bet",
                SportName = string.Empty,
                TargetCategoryName = "Smart Punter",
                TargetDate = DateTime.Now.ToString(Consts.FormatDateToString_MMMddyyyyhhmm),
                FirstMargin = 1,
                CurrentMargin = 1,
                DayLeft = 20
            });

            return list;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.ProbationManagement, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult ChangeCategory()
        {
            var data = "";

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
