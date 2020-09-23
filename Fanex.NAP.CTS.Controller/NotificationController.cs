namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.ViewModel.Notification;
    using Service;
    using System.Linq;
    using System.Web.Mvc;
    
    public class NotificationController : ConfigurationsController
    {
        private readonly INotificationService _notificationService;

        public NotificationController()
        {
            _notificationService = new NotificationService();
        }

        [NapAuth(FunctionKey = FSFunctionKeys.NotificationSettings, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public JsonResult GetAllMessages(long maxAssociationID, long maxProbationID)
        {
            var isFirstNotification = CheckFirstNotification();

            var userId = CurrentSession.UserId;
            var associatedMessages = _notificationService.GetAssociatedMessages(userId, maxAssociationID).ToList();
            var probationMessages = _notificationService.GetProbationMessages(userId, maxProbationID).ToList();

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
                associatedMaxAssociationID = (associatedUnreadMessages?.Count() > 0)? associatedMessages.Max(c=>c.MaxNewAssociationID) : 0,
                associatedUnreadCounter = associatedUnreadMessages?.Count(),

                probationMessagesCounter = probationMessages?.Count(),
                probationMaxProbationID = (probationUnreadMessages?.Count() > 0) ? probationMessages.Max(c => c.MaxNotificationID) : 0,
                probationUnreadCounter = probationUnreadMessages?.Count(),

                unReadMessageCounter = associatedUnreadMessages?.Count() + probationUnreadMessages?.Count(),
                isFirstNotification
            }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckFirstNotification()
        {
            var isFirst = AppSession.IsFirstGetDataNotification;

            if(isFirst)
            {
                AppSession.IsFirstGetDataNotification = false;
            }

            return isFirst;
        }
    }
}