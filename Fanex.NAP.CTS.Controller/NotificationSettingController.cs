namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Notification;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.Notification;
    using Service;
    using System.Linq;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
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

            if(!isAssociationPermission && !isProbationPermission)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new NotificationSettingViewModel
            {
                CTSCustId = CurrentSession.UserId,
                IsAssociationPermission = isAssociationPermission,
                IsProbationPermission = isProbationPermission,
                NotificationSettingStatus = GetNotificationSettingStatus()
            });
        }

        public JsonResult GetAllMessages(long maxAssociationID, long maxProbationID)
        {
            var isFirstNotification = CheckFirstNotification();

            var userId = CurrentSession.UserId;
            var associatedMessages = _notificationSettingService.GetAssociatedMessages(userId, maxAssociationID).ToList();
            var probationMessages = _notificationSettingService.GetProbationMessages(userId, maxProbationID).ToList();

            var associatedUnreadMessages = associatedMessages.Select(c => c).Where(x => x.IsSeen == false);
            var probationUnreadMessages = probationMessages.Select(c => c).Where(x => x.IsSeen == false);

            return Json(new
            {
                messages = new NotificationMessagesViewModel()
                {
                    AssociatedMessages = associatedMessages,
                    ProbationMessages = probationMessages
                },
                associatedMessagesCounter = associatedMessages?.Count(),
                associatedMaxAssociationID = (associatedUnreadMessages?.Count() > 0) ? associatedMessages.Max(c => c.MaxNewAssociationID) : 0,
                associatedUnreadCounter = associatedUnreadMessages?.Count(),

                probationMessagesCounter = probationMessages?.Count(),
                probationMaxProbationID = (probationUnreadMessages?.Count() > 0) ? probationMessages.Max(c => c.MaxNotificationID) : 0,
                probationUnreadCounter = probationUnreadMessages?.Count(),

                unReadMessageCounter = associatedUnreadMessages?.Count() + probationUnreadMessages?.Count(),
                isFirstNotification
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.NotificationSetting)]
        public ActionResult UpdateNotificationSettingStatus(UpdateNotifiedSettingStatusModel notifiedSettingStatusModel)
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

            if (!isAssociationPermission && !isProbationPermission)
            {
                return Json(new DBResult
                {
                    ErrorCode = Consts.ErrorMess_LosingAllFuncNotificaionSetting,
                    ErrorMsg = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }

            if((!isAssociationPermission && notifiedSettingStatusModel.FunctionId == 1)
                || !isProbationPermission && notifiedSettingStatusModel.FunctionId == 2)
            {
                return Json(new DBResult
                {
                    ErrorCode = Consts.ErrorMess_LosingFuncNotificaionSetting,
                    ErrorMsg = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }

            var result = _notificationSettingService.UpdateNotificationSetting(CurrentSession.UserId, notifiedSettingStatusModel);

            return Json(new
            {
                result.ErrorCode,
                result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        public NotificationSettingStatusViewModel GetNotificationSettingStatus()
        {
            var notificationSettingStatus = _notificationSettingService.GetNotificationSettingStatus(CurrentSession.UserId);
            bool isTurnOnAssociationStatus = false;
            bool isTurnOnProbationStatus = false;

            if (notificationSettingStatus != null && notificationSettingStatus.Any())
            {
                isTurnOnAssociationStatus = notificationSettingStatus.Where(x => x.FunctionID == 1).Select(c => c.IsTurnedOnNotification).SingleOrDefault();
                isTurnOnProbationStatus = notificationSettingStatus.Where(x => x.FunctionID == 2).Select(c => c.IsTurnedOnNotification).SingleOrDefault();
            }

            return new NotificationSettingStatusViewModel
            {
                IsTurnOnAssociationStatus = isTurnOnAssociationStatus,
                IsTurnOnProbationStatus = isTurnOnProbationStatus
            };
        }

        private bool CheckFirstNotification()
        {
            var isFirst = AppSession.IsFirstGetDataNotification;

            if (isFirst)
            {
                AppSession.IsFirstGetDataNotification = false;
            }

            return isFirst;
        }
    }
}