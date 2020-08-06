namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core.BaseClasses;    
    using Fanex.NAP.CTS.Model.DTO;
    using Criteria.Notification;
    using System.Collections.Generic;

    public class NotificationService : INotificationService
    {
        public IEnumerable<Message> Get(int custId, int maxAssociationID)
        {
            return DataService.GetData<Message>(new UserMessageGetCriteria(custId, maxAssociationID));
        }
    }
}