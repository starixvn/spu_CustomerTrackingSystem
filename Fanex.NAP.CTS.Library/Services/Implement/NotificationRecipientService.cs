namespace Fanex.NAP.CTS.Library.Services.Implement
{
    using Fanex.NAP.CTS.Library.Core.BaseClasses;
    using Fanex.NAP.CTS.Library.Criteria.Notification;
    using Fanex.NAP.CTS.Library.Models;
    using Fanex.NAP.CTS.Library.Models.FraudCase;
    using Fanex.NAP.CTS.Library.Services.Interface;
    using System.Collections.Generic;
    using System.Linq;

    public class NotificationRecipientService : INotificationRecipientService
    {
        public DBResult AddRecipient(string recipients, int userId, FraudCaseId fraudCase)
        {
            var criteria = new RecipientAddCriteria
            {
                RecipientIds = recipients,
                UserId = userId,
                FraudCaseId = (int)fraudCase
            };

            return DataService.UpdateData(criteria, "Notification");
        }

        public IEnumerable<NotificationRecipient> GetRecipients(FraudCaseId fraudCase)
        {
            return DataService.GetData<NotificationRecipient>(new RecipientGetCriteria { FraudCaseId = (int)fraudCase }).OrderBy(nr => nr.Recipient);
        }

        public DBResult RemoveRecipient(int recipientId, int userId, FraudCaseId fraudCase)
        {
            var criteria = new RecipientRemoveCriteria
            {
                RecipientId = recipientId,
                UserId = userId,
                FraudCaseId = (int)fraudCase
            };

            return DataService.UpdateData(criteria, "Notification");
        }
    }
}