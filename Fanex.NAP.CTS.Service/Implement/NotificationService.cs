namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core.BaseClasses;    
    using Fanex.NAP.CTS.Model.DTO;
    using Criteria.Notification;
    using System.Collections.Generic;
    using Fanex.Data;

    public class NotificationService : INotificationService
    {
        public IEnumerable<AssociatedMessageModel> GetAssociatedMessages(int custId, long maxAssociationID)
        {
            return DataService.GetData<AssociatedMessageModel>(new AssociatedMessagesGetCriteria(custId, maxAssociationID));
        }

        public IEnumerable<ProbationMessageModel> GetProbationMessages(int custId, long maxNotificationID)
        {
            var criteria = new ProbationMessageGetCriteria(custId, maxNotificationID);

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<ProbationMessageModel>(criteria);

                return result;
            }
        }
    }
}