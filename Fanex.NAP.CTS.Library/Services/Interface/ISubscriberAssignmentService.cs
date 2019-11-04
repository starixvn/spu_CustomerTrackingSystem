namespace Fanex.NAP.CTS.Library.Services
{
    using Fanex.NAP.CTS.Library.Models;
    using Fanex.NAP.CTS.Library.Models.Subscriber;
    using System.Collections.Generic;

    public interface ISubscriberAssignmentService
    {
        IEnumerable<SubscriberAssignment> GetSubscribers(long userId);

        DBResult UpdateSubscribers(bool isMasterSub, int userId, string subscriberIds, int createdBy);

    }
}