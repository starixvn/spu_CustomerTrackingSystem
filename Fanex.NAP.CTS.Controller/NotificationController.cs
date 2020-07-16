namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
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
        public JsonResult GetAllMessages(int aMaxAssociationID)
        {
            var userId = CurrentSession.UserId;
            var messages = _notificationService.Get(userId, aMaxAssociationID).ToList();
            var unReadList = messages.Select(c => c).Where(x => x.IsSeen == false);

            return Json(new
            {
                messages,
                maxAssociationID = (unReadList?.Count() > 0)?  messages.Max(c=>c.MaxNewAssociationID) : 0,
                unReadList,
                unReadMessageCounter = unReadList?.Count(),
                messageCounter = messages?.Count()
            }, JsonRequestBehavior.AllowGet);
        }
    }
}