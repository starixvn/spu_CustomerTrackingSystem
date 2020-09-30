namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.Notification;
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

            var isAssociationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Read);

            var isProbationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                          NapSettings.Settings.SiteId,
                          FSFunctionKeys.ProbationManagement,
                          NAP.Core.Business.Constant.Policy.Read);

            return View(new NotificationSettingViewModel
            {
                IsAssociationPermission = isAssociationPermission,
                IsProbationPermission = isProbationPermission
            });
        }

    }
}