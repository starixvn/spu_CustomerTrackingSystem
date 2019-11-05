namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.Subscriber;
    using System.Collections.Generic;

    public interface ISubscriberAssignmentService
    {
        IEnumerable<SubscriberAssignment> GetSubscribers(long userId);

        DBResult UpdateSubscribers(bool isMasterSub, int userId, string subscriberIds, int createdBy);

    }
}