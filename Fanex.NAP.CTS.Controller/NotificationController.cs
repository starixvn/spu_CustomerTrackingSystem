namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Utilities;
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

        [HttpGet]
        [NapAuth(FunctionKey = FSFunctionKeys.NotificationSettings, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public JsonResult GetAllMessages(int maxAssociationID)
        {
            var isFirstNotification = CheckFirstNotification();

            var userId = CurrentSession.UserId;
            var messages = _notificationService.Get(userId, maxAssociationID).ToList();
            var unReadList = messages.Select(c => c).Where(x => x.IsSeen == false);
            var readList = messages.Select(c => c).Where(x => x.IsSeen == true);

            return Json(new
            {
                messages,
                messageCounter = messages?.Count(),
                maxAssociationID = (unReadList?.Count() > 0)?  messages.Max(c=>c.MaxNewAssociationID) : 0,
                unReadList,
                unReadMessageCounter = unReadList?.Count(),
                readList,
                readListCounter = readList?.Count(),
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