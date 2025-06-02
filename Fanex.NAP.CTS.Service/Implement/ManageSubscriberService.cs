namespace Fanex.NAP.CTS.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.ManageSubscriber;
    using Fanex.NAP.CTS.Model.ManageSubscriber;
    using Fanex.NAP.CTS.Service.Interface;
    using Newtonsoft.Json;

    public class ManageSubscriberService : IManageSubscriberService
    {
        public List<SubscriberModel> GetSubscribers()
        {
            var criteria = new SubscriberGetCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SubscriberModel>(criteria).ToList();
            }
        }

        public List<DetailedSiteModel> GetDetailedSite(long subscriberID)
        {
            var criteria = new SubscriberGetDetailedSiteCriteria()
            {
                ip_SubscriberID = subscriberID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetailedSiteModel>(criteria).ToList();
            }
        }

        public UpdateSubsciberResultModel UpdateSubscriber(SubscriberUpdateModel subscriberInsertModel, long userId)
        {
            var rs = new UpdateSubsciberResultModel
            {
                Subscribers = new List<ReUpdateSubscribeBySiteIDModel>(),
                Result = new DBResult { ErrorCode = 0, ErrorMsg = string.Empty }
            };

            var criteria = new SubscriberUpdateCriteria
            {
                SubscriberName = subscriberInsertModel.SubscriberName,
                SubscriberID = subscriberInsertModel.SubscriberID,
                SubscriberPrefix = subscriberInsertModel.IBCUserNamePrefix,
                SubscriberType = subscriberInsertModel.SubscriberType,
                SubscriberSourceID = subscriberInsertModel.SubscriberSourceID,
                IsTest = subscriberInsertModel.IsTest,
                UserID=userId,
                SiteList = subscriberInsertModel.SiteListJson

            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    rs.Subscribers = db.Query<ReUpdateSubscribeBySiteIDModel>(criteria).ToList();

                    if (!string.IsNullOrEmpty(criteria.ErrorMessage))
                    {
                        rs.Result = new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
                    }

                    return rs;
                }
                catch (Exception ex)
                {
                    LogExtension.LogError("CTS - Update Manage Subscriber throws exception: " + ex.Message, ex);

                    return rs;
                }
            }
        }

        public DBResult RemoveSubscriber(int subscriberID, long userId)
        {
            var criteria = new SubscriberRemoveCriteria
            {
                SubscriberID = subscriberID,
                UserID = userId
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
                    LogExtension.LogError("CTS - Remove Manage Subscriber throws exception: " + ex.Message, ex);

                    return new DBResult
                    {
                        ErrorCode = -1,
                        ErrorMsg = ex.Message
                    };
                }
            }
        }

        public void ReUpdateSubriberIDBySiteID(List<ReUpdateSubscribeBySiteIDModel> subSite)
        {
            var criteria = new ReUpdateSubriberIdBySiteIdCriteira
            {
                SubsSiteJson = JsonConvert.SerializeObject(subSite)
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }
    }
}
