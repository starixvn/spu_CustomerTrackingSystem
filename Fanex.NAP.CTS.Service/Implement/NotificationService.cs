namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Notification;
    using Criteria.Notification;
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.NAP.CTS.Core;
    using System;
    using System.Linq;
    using Fanex.NAP.CTS.Core.Extentions;

    public class NotificationService : INotificationService
    {
        public IEnumerable<AssociatedMessageModel> GetAssociatedMessages(long custId, long maxAssociationID)
        {
            var criteria = new AssociatedMessagesGetCriteria(custId, maxAssociationID);

            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<AssociatedMessageModel>(criteria);
                }
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
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<ProbationMessageModel>(criteria);
                }
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
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<NotifiedSettingStatusModel>(criteria);
                }
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

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);

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
}