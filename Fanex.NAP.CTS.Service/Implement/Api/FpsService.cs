namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Criteria.Api.FPS.Account;
    using Fanex.NAP.CTS.Criteria.Api.FPS.SearchAccount;
    using Fanex.NAP.CTS.Criteria.Api.FPS.Transaction;
    using Fanex.NAP.CTS.Model.API.FPS.Account;
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using Fanex.NAP.CTS.Model.API.FPS.Transaction;
    using Fanex.NAP.CTS.Service.Interface.Api;

    public class FpsService : IFpsService
    {
        private readonly IDynamicRepository repository;

        public FpsService()
        {
            repository = new DynamicRepository();            
        }

        public SearchAccountResultModel SearchAccount(SearchAccountParamModel param)
        {
            var data = new SearchAccountResultModel();

            if (param.IsSearchPartial)
            {
                var criteria = new GetListAccountByUserNameSubscriberCriteria
                {
                    SubscriberID = param.SubscriberID,
                    UserName = param.UserName,
                    Take = param.Take,
                    Skip = param.Skip
                };

                data.Items = repository.Fetch<SearchAccountItemModel>(criteria);
                data.TotalItem = criteria.TotalItem;
            } 
            else
            {
                var criteria = new GetAccountByNameSubscriberCriteria
                {
                    SubscriberID = param.SubscriberID,
                    UserNameList = param.UserName
                };

                data.Items = repository.Fetch<SearchAccountItemModel>(criteria);
            }

            return data;
        }

        public GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriber(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            var criteria = new GetAssAccountInfoByCustSubscriberCriteria
            {
                CTSCustID = param.CTSCustID,
                SubscriberID = param.SubscriberID
            };

            return DataService.GetSingle<GetAssAccountInfoByCustSubscriberModel>(criteria);
        }

        public GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber(GetAssAccountsByCustSubscriberParamModel param)
        {
            var data = new GetAssAccountsByCustSubscriberResultModel();

            var criteria = new GetAssAccountsByCustSubscriberCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<GetAssAccountsByCustSubscriberItemModel>(criteria);
            data.TotalItem = criteria.TotalItem;

            return data;
        }

        public TransactionResultModel GetTransactionHistory(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<TransactionItemModel>(criteria);
            data.TotalItem = criteria.TotalItem;

            return data;
        }
    }
}
