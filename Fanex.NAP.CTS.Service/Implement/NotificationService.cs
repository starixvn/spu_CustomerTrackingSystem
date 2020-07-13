namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core.BaseClasses;    
    using Fanex.NAP.CTS.Model.DTO;
    using Criteria.Notification;
    using System.Collections.Generic;

    public class NotificationService : INotificationService
    {
        //public int GetUnreadMessageCounter(int custId)
        //{
        //    var result = DataService.GetData<Message>(new UserMessageGetCriteria(custId));

        //    return result.UnreadMessageCounter;
        //}

        public IEnumerable<Message> Get(int custId, bool isRead)
        {
            return DataService.GetData<Message>(new UserMessageGetCriteria(custId, (isRead == true)? 1 : 0 ));
        }
    }
}