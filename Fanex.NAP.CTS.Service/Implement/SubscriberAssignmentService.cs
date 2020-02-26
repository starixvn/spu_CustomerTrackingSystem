namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Criteria.SubscriberAssignment;
    using Fanex.NAP.CTS.Model.Subscriber;
    using System.Linq;
    using System.Collections.Generic;
    using System;
    using Fanex.NAP.CTS.Core;

    public class SubscriberAssignmentService : ISubscriberAssignmentService
    {
        public IEnumerable<SubscriberAssignment> GetSubscribers(int userId)
        {
            var criteria = new GetAllSubscriberCriteria
            {
                UserId = userId
            };

            return DataService.GetData<SubscriberAssignment>(criteria)
                                .OrderBy(c => c.SubscriberName)
                                .ToList();
        }

        public DBResult UpdateSubscribers(SubscriberAssignmentUpdate subscriberAssignmentUpdate)
        {
            var criteria = new SubscribersUpdateCriteria
            {
                ip_isMasterSub = subscriberAssignmentUpdate.IsMasterSub,
                ip_subscriberIds = subscriberAssignmentUpdate.SubscriberIds,
                ip_userId = subscriberAssignmentUpdate.UserId,
                ip_createdBy = subscriberAssignmentUpdate.CreatedBy
            };

            try
            {
                return DataService.UpdateData(criteria, "Update Subscriber Assignment");
            }
            catch (Exception ex)
            {
                return new DBResult
                {
                    ErrorCode = 1,
                    ErrorMsg = ex.Message
                };
            }
        }
    }
}