namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Service;
    using System.Linq;
    using System.Web.Mvc;
    
    public class NotificationSettingController : ConfigurationsController
    {
        private readonly INotificationService _notificationSettingService;

        public NotificationSettingController()
        {
            _notificationSettingService = new NotificationService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ImportEvidences,
                            NAP.Core.Business.Constant.Policy.Host);

            var isOverwritePermission = userService.IsHavingPermission(CurrentSession.UserId,
                          NapSettings.Settings.SiteId,
                          FSFunctionKeys.OverwriteEvidences,
                          NAP.Core.Business.Constant.Policy.Read);

            return View();
        }

    }
}