namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Notification;
    using System.Collections.Generic;

    public interface INotificationService
    {
        IEnumerable<AssociatedMessageModel> GetAssociatedMessages(int custId, long maxAssociationID);

        IEnumerable<ProbationMessageModel> GetProbationMessages(int custId, long maxNotificationID);

    }
}