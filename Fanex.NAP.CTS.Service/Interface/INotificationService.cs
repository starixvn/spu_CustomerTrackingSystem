namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.Notification;
    using System.Collections.Generic;

    public interface INotificationService
    {
        IEnumerable<AssociatedMessageModel> GetAssociatedMessages(long custId, long maxAssociationID);

        IEnumerable<ProbationMessageModel> GetProbationMessages(long custId, long maxNotificationID);

        IEnumerable<NotifiedSettingStatusModel> GetNotificationSettingStatus(long custId);

        DBResult UpdateNotificationSetting(long custId, UpdateNotifiedSettingStatusModel notifiedSettingStatus);

    }
}