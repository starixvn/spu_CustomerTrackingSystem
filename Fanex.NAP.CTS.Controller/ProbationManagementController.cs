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
    using System.Linq;

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
        public ActionResult GetProbationCategory(string username, int dayLeft, decimal minMargin, decimal maxMargin, int categoryMode)
        {
            var viewModel = Enumerable.Empty<ProbationCategoryViewModel>();

            var data = _probationService.GetProbationCategory(username, dayLeft, categoryMode);

            if (data != null && data.Any())
            {
                viewModel = data.Select(x => new ProbationCategoryViewModel
                {
                    CustId = x.CustID,
                    UserName = x.Username,
                    SiteName = x.Site,
                    SportGroupId = x.SportGroupID,
                    SportName = x.Sport,
                    TargetCategoryName = x.TargetCategory,
                    TaggedDate = x.TaggedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmm),
                    FirstMargin = x.FirstMargin,
                    CurrentMargin = x.CurrentMargin,
                    DaysLeft = x.DaysLeft
                });
            }

            return Json(new
            {
                data = viewModel
            }, JsonRequestBehavior.AllowGet);
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
