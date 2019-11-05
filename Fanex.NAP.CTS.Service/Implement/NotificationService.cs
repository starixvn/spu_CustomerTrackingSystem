namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core.BaseClasses;    
    using Fanex.NAP.CTS.Model.DTO;
    using Criteria.Notification;
    using System.Collections.Generic;

    public class NotificationService : INotificationService
    {
        public int GetUnreadMessageCounter(int custId)
        {
            var criteria = new UserMessageUnreadCounterGetCriteria(custId);

            var result = DataService.GetSingle<dynamic>(criteria);

            return result.UnreadMessageCounter;
        }

        public IEnumerable<Message> Get(int custId)
        {
            return DataService.GetData<Message>(new UserMessageGetCriteria(custId));
        }
    }
}