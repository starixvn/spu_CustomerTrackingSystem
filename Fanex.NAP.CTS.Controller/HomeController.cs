namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.Home;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;

    [NapSuperAuth]
    public class HomeController : BaseController
    {
        private readonly IUserService userService;

        public HomeController()
        {
            userService = new UserService();
        }

        [HttpGet]
        public void test()
        {
            LogService.Info($"CTS test site");
        }

        public ActionResult Index()
        {
            ViewBag.UserId = CurrentSession.UserId;
            ViewBag.OnUserId = CurrentSession.OnUserId;
            ViewBag.IsAssociationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Read);

            ViewBag.IsProbationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                       NapSettings.Settings.SiteId,
                       FSFunctionKeys.ProbationManagement,
                       NAP.Core.Business.Constant.Policy.Read);

            var model = new AIInsightsFileUploadModel();

            return View(model);
        }

        public override string BaseControllerName { get => Consts.MenuItemHome; }

        public ActionResult Menu(string activeMenu)
        {
            ViewBag.ActiveMenu = activeMenu ?? this.BaseControllerName;

            return PartialView("~/Views/Shared/_Menu.cshtml", CurrentSession.MenuItemList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UpdateMenuSettingForLoginPartialPage()
        {
            ViewBag.IsAssociationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Read);

            ViewBag.IsProbationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                       NapSettings.Settings.SiteId,
                       FSFunctionKeys.ProbationManagement,
                       NAP.Core.Business.Constant.Policy.Read);

            return PartialView("~/Views/Shared/_LoginPartial.cshtml");
        }

        public ActionResult UpdateMenuSettingForHomePartialPage()
        {
            ViewBag.IsAssociationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Read);

            ViewBag.IsProbationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                       NapSettings.Settings.SiteId,
                       FSFunctionKeys.ProbationManagement,
                       NAP.Core.Business.Constant.Policy.Read);

            return PartialView("~/Views/Shared/_HomeLoginPartial.cshtml");
        }
    }
}