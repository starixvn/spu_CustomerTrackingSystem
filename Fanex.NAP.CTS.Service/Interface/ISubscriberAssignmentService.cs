namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.Subscriber;
    using System.Collections.Generic;

    public interface ISubscriberAssignmentService
    {
        IEnumerable<SubscriberAssignment> GetSubscribers(int userId);

        DBResult UpdateSubscribers(SubscriberAssignmentUpdate subscriberAssignmentUpdate);
    }
}