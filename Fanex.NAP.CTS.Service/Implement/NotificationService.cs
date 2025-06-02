namespace Fanex.NAP.CTS.Service
{
    using System;
    using Fanex.NAP.CTS.Model.Notification;
    using Criteria.Notification;
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Core.Extentions;

    public class NotificationService : INotificationService
    {
        private readonly ICriteriaService _repository;

        public NotificationService()
        {
            _repository = new CriteriaService();
        }

        public IEnumerable<AssociatedMessageModel> GetAssociatedMessages(long custId, long maxAssociationID)
        {
            var criteria = new AssociatedMessagesGetCriteria(custId, maxAssociationID);

            try
            {
                return _repository.Fetch<AssociatedMessageModel>(criteria);
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Notification - Get Association Messages exception: " + ex.Message;

                LogExtension.LogError(errorMessage, ex);

                return new List<AssociatedMessageModel>();
            }
        }

        public IEnumerable<ProbationMessageModel> GetProbationMessages(long custId, long maxNotificationID)
        {
            var criteria = new ProbationMessageGetCriteria(custId, maxNotificationID);

            try
            {
                return _repository.Fetch<ProbationMessageModel>(criteria);
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Notification - Get Probation Messages exception: " + ex.Message;

                LogExtension.LogError(errorMessage, ex);

                return new List<ProbationMessageModel>();
            }
        }

        public IEnumerable<NotifiedSettingStatusModel> GetNotificationSettingStatus(long custId)
        {
            var criteria = new NotificationSettingStatusGetCriteria
            {
                UserID = custId
            };

            try
            {
                return _repository.Fetch<NotifiedSettingStatusModel>(criteria);
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Notification Setting - Get Status exception: " + ex.Message;

                LogExtension.LogError(errorMessage, ex);

                return new List<NotifiedSettingStatusModel>();
            }
        }

        public DBResult UpdateNotificationSetting(long custId, UpdateNotifiedSettingStatusModel notifiedSettingStatus)
        {
            var criteria = new NotificationSettingStatusUpdateCriteria
            {
                UserID = custId,
                FunctionID = notifiedSettingStatus.FunctionId,
                IsTurningOnPermission = notifiedSettingStatus.IsTurningOnPermission
            };

            try
            {
                _repository.Execute(criteria);

                if (!string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    return new DBResult
                    {
                        ErrorCode = -1,
                        ErrorMsg = criteria.ErrorMessage
                    };
                }
                else
                {
                    return new DBResult
                    {
                        ErrorCode = 0,
                        ErrorMsg = string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - Notification Setting - Update Status throws exception: " + ex.Message, ex);

                return new DBResult
                {
                    ErrorCode = -1,
                    ErrorMsg = ex.Message
                };
            }
        }
    }
}