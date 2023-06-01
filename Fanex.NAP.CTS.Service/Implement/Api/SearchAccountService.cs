namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Api.SearchAccount;
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using System;
    using System.Collections.Generic;

    public class SearchAccountService : ISearchAccountService
    {
        private readonly IDynamicRepository repository;

        public SearchAccountService()
        {
            repository = new DynamicRepository();            
        }

        public IEnumerable<SearchAccountResultModel> SearchAccount(int subscriberID, string userNameList)
        {
            var criteria = new GetAccountByNameInSubscriberCriteria
            {
                 SubscriberID = subscriberID,
                 UserNameList = userNameList
            };

            return repository.Fetch<SearchAccountResultModel>(criteria);
        }

        public IEnumerable<SearchAccountResultModel> SearchPartialAccount(int subscriberID, string userNameList)
        {
            var criteria = new GetAccountByNameInSubscriberCriteria
            {
                SubscriberID = subscriberID,
                UserNameList = userNameList
            };

            return repository.Fetch<SearchAccountResultModel>(criteria);
        }

    }
}
