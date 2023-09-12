namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Criteria.Common;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommonService : ICommonService
    {
        public IEnumerable<SubscriberGroupViewModel> GetSubscriberGroups()
        {
            var subscriberGroups = Enumerable.Empty<SubscriberGroupViewModel>();
            var data = GetSubscribers();

            if (data != null)
            {
                subscriberGroups = data
                    .GroupBy(g => new { g.SubscriberGroupID, g.SubscriberGroupName })
                    .Select(x => new { Group = x.Key, SubscriberList = x.ToList() })
                    .Select(x => new SubscriberGroupViewModel
                    {
                        SubscriberGroupID = x.Group.SubscriberGroupID,
                        SubscriberGroupName = x.Group.SubscriberGroupName,
                        SubscriberList = x.SubscriberList.Select(y => new SubscriberViewModel
                        {
                            SubscriberID = y.SubscriberID,
                            SubscriberName = y.SubscriberName
                        })
                    });
            }

            return subscriberGroups;
        }

        public IEnumerable<SubscriberGroupModel> GetSubscribers()
        {
            var criteria = new GetSubscriberGroupCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SubscriberGroupModel>(criteria);
            }
        }

        public IEnumerable<ValidateUserNameModel> ValidateUsernames(string userNames, out bool isException)
        {
            isException = false;

            var criteria = new ValidateUsernameCriteria
            {
                userNames = userNames
            };

            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<ValidateUserNameModel>(criteria);
                }
            }
            catch (Exception ex)
            {
                isException = true;
                LogService.Error(ex.Message, ex);

                return Enumerable.Empty<ValidateUserNameModel>();
            }
        }

        public IEnumerable<SiteModel> GetSites(bool isNestedCalled)
        {
            var criteria = new GetSitesCriteria { IsNestedCalled = isNestedCalled };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SiteModel>(criteria);
            }
        }

        public IEnumerable<MainMatchModel> GetMainMatchInfosByIDs(string matchIDs)
        {
            var criteria = new GetMatchesInfoCriteria
            {
                MatchIds = matchIDs
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MainMatchModel>(criteria);
            }
        }

        public IEnumerable<MainTicketDetailModel> GetMainTicketDetails(string transIds, int? transSize, int? matchSize)
        {           
            var criteria = new GetTicketDetailsCriteria
            {
                ListTransId = transIds,
                TransSize = transSize,
                MatchSize = matchSize
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MainTicketDetailModel>(criteria);
            }
        }

        public IEnumerable<CommonCustomerClassModel> GetCommonCustomerClass()
        {
            var criteria = new GetCommonCustomerClassCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CommonCustomerClassModel>(criteria);
            }
        }

    }
}
