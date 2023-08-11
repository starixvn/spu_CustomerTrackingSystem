namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Criteria.Api.FPS.Account;
    using Fanex.NAP.CTS.Criteria.Api.FPS.Device;
    using Fanex.NAP.CTS.Criteria.Api.FPS.Search;
    using Fanex.NAP.CTS.Criteria.Api.FPS.SearchAccount;
    using Fanex.NAP.CTS.Criteria.Api.FPS.Transaction;
    using Fanex.NAP.CTS.Model.API.FPS.Account;
    using Fanex.NAP.CTS.Model.API.FPS.Device;
    using Fanex.NAP.CTS.Model.API.FPS.Search;
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

        #region cts

        public SearchAccountResultModel SearchAccount_DCSCenter(SearchAccountParamModel param)
        {
            var data = new SearchAccountResultModel();

            if (param.IsSearchPartial)
            {
                var criteria = new GetListAccountByUserNameSubscriberDCSCenterCriteria
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
                var criteria = new GetAccountByNameSubscriberDCSCenterCriteria
                {
                    SubscriberID = param.SubscriberID,
                    UserName = param.UserName
                };

                data.Items = repository.Fetch<SearchAccountItemModel>(criteria);
            }

            return data;
        }

        public GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriber_DCSCenter(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            var criteria = new GetAssAccountInfoByCustSubscriberDCSCenterCriteria
            {
                CTSCustID = param.CTSCustID,
                SubscriberID = param.SubscriberID
            };

            return DataService.GetSingle<GetAssAccountInfoByCustSubscriberModel>(criteria);
        }

        public GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber_DCSCenter(GetAssAccountsByCustSubscriberParamModel param)
        {
            var data = new GetAssAccountsByCustSubscriberResultModel();

            var criteria = new GetAssAccountsByCustSubscriberDCSCenterCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssAccountsByCustSubscriberItemModel>(criteria);

            return data;
        }

        public GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriber_DCSCenter(GetAssDevicesByCustSubscriberParamModel param)
        {
            var data = new GetAssDevicesByCustSubscriberResultModel();

            var criteria = new GetAssDevicesByCustSubscriberDCSCenterCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssDevicesByCustSubscriberItemModel>(criteria);

            return data;
        }

        public GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriber_DCSCenter(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            var data = new GetAssMatrixAccountByCustSubscriberResultModel();
            var criteria = new GetAssMatrixAccountByCustSubscriberDCSCenterCriteria()
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfCustomer = param.NoOfCustomer,
                NoOfDevice = param.NoOfDevice
            };

            data.Items = repository.Fetch<GetAssMatrixAccountByCustSubscriberItemModel>(criteria);

            return data;
        }

        public TransactionResultModel GetTransactionHistory_DCSCenter(TransactionParamModel param)
        {
            if (param.CTSCustID != null && param.CTSCustID > 0)
            {
                return GetTransactionByCustSubscriber_DCSCenter(param);
            }

            return GetTransactionBySubscriber_DCSCenter(param);
        }

        private TransactionResultModel GetTransactionByCustSubscriber_DCSCenter(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryByCustSubscriberDCSCenterCriteria
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

        private TransactionResultModel GetTransactionBySubscriber_DCSCenter(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryBySubscriberDCSCenterCriteria
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

        public LastestTransactionResultModel GetLastestTransactionBySubscriber_DCSCenter(LastestTransactionParamModel param)
        {
            var data = new LastestTransactionResultModel();

            var criteria = new GetLastestTransactionDCSCenterCriteria
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
        #endregion cts

        #region dcs

        #region search
        public SearchAccountResultModel SearchAccount_DCSExtra(SearchAccountParamModel param)
        {
            var data = new SearchAccountResultModel();
            var items = default(IEnumerable<SearchAccountItemDCSExtraModel>);

            if (param.IsSearchPartial)
            {
                var criteria = new GetListAccountByUserNameSubscriberDCSExtraCriteria
                {
                    SubscriberID = param.SubscriberID,
                    UserName = param.UserName,
                    Take = param.Take,
                    Skip = param.Skip
                };

                items = repository.Fetch<SearchAccountItemDCSExtraModel>(criteria);
                data.TotalItem = criteria.TotalItem;
            }
            else
            {
                var criteria = new GetAccountByNameSubscriberDCSExtraCriteria
                {
                    SubscriberID = param.SubscriberID,
                    UserName = param.UserName
                };

                items = repository.Fetch<SearchAccountItemDCSExtraModel>(criteria);
            }

            data.Items = items.Select(x => new SearchAccountItemModel()
            {
                CTSCustID = x.AccountID,
                SubscriberID = x.SubscriberID,
                Username = x.LoginName
            });

            return data;
        }

        public IEnumerable<SearchDeviceItemDCSExtraModel> SearchDevice_DCSExtra(SearchDeviceParamModel param)
        {
            var criteria = new GetListDeviceByCodeSubscriberDCSExtraCriteria
            {
                SubscriberID = param.SubscriberID,
                DeviceCode = param.DeviceCode
            };

            var items = repository.Fetch<SearchDeviceItemDCSExtraModel>(criteria);
            return items;
        }
        #endregion

        #region account
        public GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriber_DCSExtra(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            var criteria = new GetAssAccountInfoByCustSubscriberDCSExtraCriteria
            {
                CTSCustID = param.CTSCustID,
                SubscriberID = param.SubscriberID
            };

            return DataService.GetSingle<GetAssAccountInfoByCustSubscriberModel>(criteria);
        }

        public GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber_DCSExtra(GetAssAccountsByCustSubscriberParamModel param)
        {
            var data = new GetAssAccountsByCustSubscriberResultModel();

            var criteria = new GetAssAccountsByCustSubscriberDCSExtraCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssAccountsByCustSubscriberItemDcsModel>(criteria).Select(x => new GetAssAccountsByCustSubscriberItemModel
            {
                AssociatedAccount = x.AssociatedAccount,
                CTSCustID = x.AccountID,
                DeviceInCommon = x.DeviceInCommon,
                OtherDeives = x.OtherDeives,
                UserName = x.LoginName
            });

            return data;
        }

        public GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriber_DCSExtra(GetAssDevicesByCustSubscriberParamModel param)
        {
            var data = new GetAssDevicesByCustSubscriberResultModel();

            var criteria = new GetAssDevicesByCustSubscriberDCSExtraCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssDevicesByCustSubscriberItemModel>(criteria);

            return data;
        }

        public GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriber_DCSExtra(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            var data = new GetAssMatrixAccountByCustSubscriberResultModel();
            var criteria = new GetAssMatrixAccountByCustSubscriberDCSExtraCriteria()
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfCustomer = param.NoOfCustomer,
                NoOfDevice = param.NoOfDevice
            };

            data.Items = repository.Fetch<GetAssMatrixAccountByCustSubscriberItemDCSExtraModel>(criteria).Select(x => new GetAssMatrixAccountByCustSubscriberItemModel
            {
                CreatedTime = x.CreatedTime,
                CTSCustID = x.AccountID,
                DeviceID = x.DeviceID,
                FirstDeviceCode = x.FirstDeviceCode,
                ID = x.ID,
                UserName = x.LoginName
            });

            return data;
        }
        #endregion

        #region device
        public GetAssDeviceInfoByIDSubscriberModel GetAssDeviceInfoByIDSubscriber_DCSExtra(GetAssDeviceInfoByIDSubscriberParamModel param)
        {
            var criteria = new GetAssDeviceInfoByIDSubscriberDCSExtraCriteria()
            {
                DeviceID = param.DeviceID,
                SubscriberID = param.SubscriberID,
            };

            var data = repository.FetchMultiple<GetAssDeviceInfoByIDSubscriberModel, GetAssDeviceFirstSeen>(criteria);
            var result = data.firstResultSet.FirstOrDefault() ?? new GetAssDeviceInfoByIDSubscriberModel();
            result.FirstSeenBySubscriber = data.secondResultSet;

            return result;
        }

        public IEnumerable<GetAssAccByDevSubscriberItemModel> GetAssAccByDevSubscriber_DCSExtra(GetAssAccByDevSubscriberParamModel param)
        {
            var criteria = new GetAssAccByDevSubscriberDCSExtraCriteria
            {
                DeviceID = param.DeviceID,
                NoOfRecord = param.NoOfRecord,
                SubscriberID = param.SubscriberID,
            };

            return repository.Fetch<GetAssAccByDevSubscriberItemModel>(criteria);
        }

        public IEnumerable<GetAssDevByDevSubscriberItemModel> GetAssDevByDevSubscriber_DCSExtra(GetAssDevByDevSubscriberParamModel param)
        {
            var criteria = new GetAssDevByDevSubscriberDCSExtraCriteria
            {
                DeviceID = param.DeviceID,
                NoOfRecord = param.NoOfRecord,
                SubscriberID = param.SubscriberID,
            };

            return repository.Fetch<GetAssDevByDevSubscriberItemModel>(criteria);
        }

        public IEnumerable<GetAssMatrixByDeviceSubscriberItemModel> GetAssMatrixByDeviceSubscriber_DCSExtra(GetAssMatrixByDeviceSubscriberParamModel param)
        {
            var criteria = new GetAssMatrixByDeviceSubsriberDCSExtraCriteria
            {
                DeviceID = param.DeviceID,
                NoOfCustomer = param.NoOfCustomer,
                NoOfDevice = param.NoOfDevice,
                SubscriberID = param.SubscriberID,
            };

            return repository.Fetch<GetAssMatrixByDeviceSubscriberItemModel>(criteria);
        }
        #endregion

        #region report
        public TransactionHistoryDCSExtraResultModel GetTransactionHistoryByDevice_DCSExtra(TransactionHistoryByDeviceParamModel param)
        {
            var criteria = new GetTransHistoryByDevSubDCSExtraCriteria
            {
                DeviceID = param.DeviceID,
                FromDate = param.FromDate,
                SubscriberID = param.SubscriberID,
                ToDate = param.ToDate,
                Skip = param.Skip,
                Take = param.Take,
            };

            return new TransactionHistoryDCSExtraResultModel()
            {
                Items = repository.Fetch<TransactionHistoryDCSExtraItemModel>(criteria),
                TotalItem = criteria.TotalItem
            };
        }

        public TransactionResultModel GetTransactionHistory_DCSExtra(TransactionParamModel param)
        {
            if (param.CTSCustID != null && param.CTSCustID > 0)
            {
                return GetTransactionByCustSubscriber_DCSExtra(param);
            }

            return GetTransactionBySubscriber_DCSExtra(param);
        }

        private TransactionResultModel GetTransactionByCustSubscriber_DCSExtra(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryByCustSubscriberDCSExtraCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<TransactionDCSExtraItemModel>(criteria).Select(x => new TransactionItemModel()
            {
                Action = x.Action,
                ActionResult = x.ActionResult,
                Browser = x.Browser,
                CTSCustID = x.AccountID,
                FirstDeviceCode = x.FirstDeviceCode,
                IP = x.IP,
                OS = x.OS,
                UserName = x.LoginName,
                RobotTracking = x.RobotTracking,
                SubscriberID = x.SubscriberID,
                TransID = x.TransID,
                TransTime = x.TransTime,
                URLDetails = x.URLDetails
            });
            data.TotalItem = criteria.TotalItem;

            return data;
        }

        private TransactionResultModel GetTransactionBySubscriber_DCSExtra(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryBySubscriberDCSExtraCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                SubscriberID = param.SubscriberID,
                UserName = param.UserName,
                IsPartial = param.IsPartial,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<TransactionDCSExtraItemModel>(criteria).Select(x => new TransactionItemModel()
            {
                Action = x.Action,
                ActionResult = x.ActionResult,
                Browser = x.Browser,
                CTSCustID = x.AccountID,
                FirstDeviceCode = x.FirstDeviceCode,
                IP = x.IP,
                OS = x.OS,
                UserName = x.LoginName,
                RobotTracking = x.RobotTracking,
                SubscriberID = x.SubscriberID,
                TransID = x.TransID,
                TransTime = x.TransTime,
                URLDetails = x.URLDetails
            });
            data.TotalItem = criteria.TotalItem;

            return data;
        }

        public LastestTransactionResultModel GetLastestTransactionBySubscriber_DCSExtra(LastestTransactionParamModel param)
        {
            var data = new LastestTransactionResultModel();

            var criteria = new GetLastestTransactionDCSExtraCriteria
            {
                SubscriberID = param.SubscriberID,
                Length = param.Length,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<LastestTransactionDcsItemModel>(criteria).Select(x => new LastestTransactionItemModel()
            {
                Action = x.Action,
                ActionResult = x.ActionResult,
                Browser = x.Browser,
                CTSCustID = x.AccountID,
                FirstDeviceCode = x.FirstDeviceCode,
                IP = x.IP,
                OS = x.OS,
                RobotTracking = x.RobotTracking,
                SubscriberID = x.SubscriberID,
                TransID = x.TransID,
                TransTime = x.TransTime,
                URLDetails = x.URLDetails,
                UserName = x.LoginName
            });
            data.TotalItem = criteria.TotalItem;

            return data;
        }
        #endregion

        #endregion dcs
    }
}
