namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Criteria.Notification;
    using Fanex.NAP.CTS.Model.FraudCase;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Core;
    using System.Collections.Generic;
    using System.Linq;

    public class NotificationRecipientService : INotificationRecipientService
    {
        public DBResult AddRecipient(string recipients, int userId, Model.FraudCaseId fraudCase)
        {
            var criteria = new RecipientAddCriteria
            {
                RecipientIds = recipients,
                UserId = userId,
                FraudCaseId = (int)fraudCase
            };

            return DataService.UpdateData(criteria, "Notification");
        }

        public IEnumerable<NotificationRecipient> GetRecipients(Model.FraudCaseId fraudCase)
        {
            return DataService.GetData<NotificationRecipient>(new RecipientGetCriteria { FraudCaseId = (int)fraudCase }).OrderBy(nr => nr.Recipient);
        }

        public DBResult RemoveRecipient(int recipientId, int userId, Model.FraudCaseId fraudCase)
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