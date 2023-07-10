namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Criteria.Api.FPS.Account;
    using Fanex.NAP.CTS.Criteria.Api.FPS.Device;
    using Fanex.NAP.CTS.Criteria.Api.FPS.SearchAccount;
    using Fanex.NAP.CTS.Criteria.Api.FPS.Transaction;
    using Fanex.NAP.CTS.Model.API.FPS.Account;
    using Fanex.NAP.CTS.Model.API.FPS.Device;
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using Fanex.NAP.CTS.Model.API.FPS.Transaction;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using System.Collections.Generic;
    using System.Linq;

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
                    UserName = param.UserName
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

        public GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriber(GetAssDevicesByCustSubscriberParamModel param)
        {
            var data = new GetAssDevicesByCustSubscriberResultModel();

            var criteria = new GetAssDevicesByCustSubscriberCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssDevicesByCustSubscriberItemModel>(criteria);

            return data;
        }

        public GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriber(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            var data = new GetAssMatrixAccountByCustSubscriberResultModel();
            var criteria = new GetAssMatrixAccountByCustSubscriberCriteria()
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssMatrixAccountByCustSubscriberItemModel>(criteria);

            return data;
        }

        public TransactionResultModel GetTransactionHistory(TransactionParamModel param)
        {
            if (param.CTSCustID != null && param.CTSCustID > 0)
            {
                return GetTransactionByCustSubscriber(param);
            }

            return GetTransactionBySubscriber(param);
        }

        private TransactionResultModel GetTransactionByCustSubscriber(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryByCustSubscriberCriteria
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

        private TransactionResultModel GetTransactionBySubscriber(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryBySubscriberCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                SubscriberID = param.SubscriberID,
                UserName = param.UserName,
                IsPartial = param.IsPartial,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<TransactionItemModel>(criteria);
            data.TotalItem = criteria.TotalItem;

            return data;
        }

        public LastestTransactionResultModel GetLastestTransactionBySubscriber(LastestTransactionParamModel param)
        {
            var data = new LastestTransactionResultModel();

            var criteria = new GetLastestTransactionCriteria
            {
                SubscriberID = param.SubscriberID,
                Length = param.Length,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<LastestTransactionItemModel>(criteria);
            data.TotalItem = criteria.TotalItem;

            return data;
        }
    }
}
