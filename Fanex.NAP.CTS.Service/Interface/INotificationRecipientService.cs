namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.FraudCase;
    using System.Collections.Generic;

    public interface INotificationRecipientService
    {
        IEnumerable<NotificationRecipient> GetRecipients(FraudCaseId fraudCase);

        DBResult AddRecipient(string recipients, int userId, FraudCaseId fraudCase);

        DBResult RemoveRecipient(int recipientId, int userId, FraudCaseId fraudCase);
    }
}