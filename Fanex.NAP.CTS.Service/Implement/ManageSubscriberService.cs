namespace Fanex.NAP.CTS.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Logging;
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

        public DBResult InsertSubscriber(SubscriberInsertModel subscriberInsertModel, long userId)
        {
            var criteria = new SubscriberInsertCriteria
            {
                SubscriberList = JsonConvert.SerializeObject(new List<SubscriberInsertModel>() { subscriberInsertModel }),
                UserID = userId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);

                    return new DBResult
                    {
                        ErrorCode = 0,
                        ErrorMsg = string.Empty
                    };
                }
                catch (Exception ex)
                {
                    LogService.Error("CTS - Insert Manage Subscriber throws exception: " + ex.Message, ex);

                    return new DBResult
                    {
                        ErrorCode = -1,
                        ErrorMsg = ex.Message
                    };
                }
            }
        }

        public List<ValidateSubscriberPrefixModel> VerifySubscriberToInsert(string prefixSubscriberName)
        {
            var criteria = new SubscriberPrefixVerifyCriteria()
            {
                SubscriberPrefix = prefixSubscriberName
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<ValidateSubscriberPrefixModel>(criteria);

                return result.ToList();
            }
        }

        public DBResult UpdateSubscriber(SubscriberInsertModel subscriberInsertModel)
        {
            var criteria = new SubscriberUpdateCriteria
            {
                SubscriberList = JsonConvert.SerializeObject(new List<SubscriberInsertModel>() { subscriberInsertModel })
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);

                    return new DBResult
                    {
                        ErrorCode = 0,
                        ErrorMsg = string.Empty
                    };
                }
                catch (Exception ex)
                {
                    LogService.Error("CTS - Update Manage Subscriber throws exception: " + ex.Message, ex);

                    return new DBResult
                    {
                        ErrorCode = -1,
                        ErrorMsg = ex.Message
                    };
                }
            }
        }

        public DBResult RemoveSubscriber(int subscriberID, int siteID)
        {
            var criteria = new SubscriberRemoveCriteria
            {
                SubscriberID = subscriberID,
                SiteID = siteID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);

                    return new DBResult
                    {
                        ErrorCode = 0,
                        ErrorMsg = string.Empty
                    };
                }
                catch (Exception ex)
                {
                    LogService.Error("CTS - Remove Manage Subscriber throws exception: " + ex.Message, ex);

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
