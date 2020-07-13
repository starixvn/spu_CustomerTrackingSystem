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
        public JsonResult GetAllMessages(bool isRead)
        {
            var userId = CurrentSession.UserId;
            var messages = _notificationService.Get(userId, isRead).ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }
    }
}