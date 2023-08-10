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

        public SearchAccountResultModel SearchAccountCts(SearchAccountParamModel param)
        {
            var data = new SearchAccountResultModel();

            if (param.IsSearchPartial)
            {
                var criteria = new GetListAccountByUserNameSubscriberCtsCriteria
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
                var criteria = new GetAccountByNameSubscriberCtsCriteria
                {
                    SubscriberID = param.SubscriberID,
                    UserName = param.UserName
                };

                data.Items = repository.Fetch<SearchAccountItemModel>(criteria);
            }

            return data;
        }

        public GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriberCts(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            var criteria = new GetAssAccountInfoByCustSubscriberCtsCriteria
            {
                CTSCustID = param.CTSCustID,
                SubscriberID = param.SubscriberID
            };

            return DataService.GetSingle<GetAssAccountInfoByCustSubscriberModel>(criteria);
        }

        public GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriberCts(GetAssAccountsByCustSubscriberParamModel param)
        {
            var data = new GetAssAccountsByCustSubscriberResultModel();

            var criteria = new GetAssAccountsByCustSubscriberCtsCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssAccountsByCustSubscriberItemModel>(criteria);

            return data;
        }

        public GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriberCts(GetAssDevicesByCustSubscriberParamModel param)
        {
            var data = new GetAssDevicesByCustSubscriberResultModel();

            var criteria = new GetAssDevicesByCustSubscriberCtsCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssDevicesByCustSubscriberItemModel>(criteria);

            return data;
        }

        public GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriberCts(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            var data = new GetAssMatrixAccountByCustSubscriberResultModel();
            var criteria = new GetAssMatrixAccountByCustSubscriberCtsCriteria()
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfCustomer = param.NoOfCustomer,
                NoOfDevice = param.NoOfDevice
            };

            data.Items = repository.Fetch<GetAssMatrixAccountByCustSubscriberItemModel>(criteria);

            return data;
        }

        public TransactionResultModel GetTransactionHistoryCts(TransactionParamModel param)
        {
            if (param.CTSCustID != null && param.CTSCustID > 0)
            {
                return GetTransactionByCustSubscriberCts(param);
            }

            return GetTransactionBySubscriberCts(param);
        }

        private TransactionResultModel GetTransactionByCustSubscriberCts(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryByCustSubscriberCtsCriteria
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

        private TransactionResultModel GetTransactionBySubscriberCts(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryBySubscriberCtsCriteria
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

        public LastestTransactionResultModel GetLastestTransactionBySubscriberCts(LastestTransactionParamModel param)
        {
            var data = new LastestTransactionResultModel();

            var criteria = new GetLastestTransactionCtsCriteria
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
        public SearchAccountResultModel SearchAccountDcs(SearchAccountParamModel param)
        {
            var data = new SearchAccountResultModel();
            var items = default(IEnumerable<SearchAccountItemDcsModel>);

            if (param.IsSearchPartial)
            {
                var criteria = new GetListAccountByUserNameSubscriberDcsCriteria
                {
                    SubscriberID = param.SubscriberID,
                    UserName = param.UserName,
                    Take = param.Take,
                    Skip = param.Skip
                };

                items = repository.Fetch<SearchAccountItemDcsModel>(criteria);
                data.TotalItem = criteria.TotalItem;
            }
            else
            {
                var criteria = new GetAccountByNameSubscriberDcsCriteria
                {
                    SubscriberID = param.SubscriberID,
                    UserName = param.UserName
                };

                items = repository.Fetch<SearchAccountItemDcsModel>(criteria);
            }

            data.Items = items.Select(x => new SearchAccountItemModel()
            {
                CTSCustID = x.AccountID,
                SubscriberID = x.SubscriberID,
                Username = x.LoginName
            });

            return data;
        }

        public IEnumerable<SearchDeviceItemDcsModel> SearchDeviceDcs(SearchDeviceParamModel param)
        {
            var criteria = new GetListDeviceByCodeSubscriberDcsCriteria
            {
                SubscriberID = param.SubscriberID,
                DeviceCode = param.DeviceCode
            };

            var items = repository.Fetch<SearchDeviceItemDcsModel>(criteria);
            return items;
        }
        #endregion

        #region account
        public GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriberDcs(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            var criteria = new GetAssAccountInfoByCustSubscriberDcsCriteria
            {
                CTSCustID = param.CTSCustID,
                SubscriberID = param.SubscriberID
            };

            return DataService.GetSingle<GetAssAccountInfoByCustSubscriberModel>(criteria);
        }

        public GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriberDcs(GetAssAccountsByCustSubscriberParamModel param)
        {
            var data = new GetAssAccountsByCustSubscriberResultModel();

            var criteria = new GetAssAccountsByCustSubscriberDcsCriteria
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

        public GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriberDcs(GetAssDevicesByCustSubscriberParamModel param)
        {
            var data = new GetAssDevicesByCustSubscriberResultModel();

            var criteria = new GetAssDevicesByCustSubscriberDcsCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssDevicesByCustSubscriberItemModel>(criteria);

            return data;
        }

        public GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriberDcs(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            var data = new GetAssMatrixAccountByCustSubscriberResultModel();
            var criteria = new GetAssMatrixAccountByCustSubscriberDcsCriteria()
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfCustomer = param.NoOfCustomer,
                NoOfDevice = param.NoOfDevice
            };

            data.Items = repository.Fetch<GetAssMatrixAccountByCustSubscriberItemDcsModel>(criteria).Select(x => new GetAssMatrixAccountByCustSubscriberItemModel
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
        public GetAssDeviceInfoByIDSubscriberModel GetAssDeviceInfoByIDSubscriberDcs(GetAssDeviceInfoByIDSubscriberParamModel param)
        {
            var criteria = new GetAssDeviceInfoByIDSubscriberDcsCriteria()
            {
                DeviceID = param.DeviceID,
                SubscriberID = param.SubscriberID,
            };

            var data = repository.FetchMultiple<GetAssDeviceInfoByIDSubscriberModel, GetAssDeviceFirstSeen>(criteria);
            var result = data.firstResultSet.FirstOrDefault() ?? new GetAssDeviceInfoByIDSubscriberModel();
            result.FirstSeenBySubscriber = data.secondResultSet;

            return result;
        }

        public IEnumerable<GetAssAccByDevSubscriberItemModel> GetAssAccByDevSubscriberDcs(GetAssAccByDevSubscriberParamModel param)
        {
            var criteria = new GetAssAccByDevSubscriberDcsCriteria
            {
                DeviceID = param.DeviceID,
                NoOfRecord = param.NoOfRecord,
                SubscriberID = param.SubscriberID,
            };

            return repository.Fetch<GetAssAccByDevSubscriberItemModel>(criteria);
        }

        public IEnumerable<GetAssDevByDevSubscriberItemModel> GetAssDevByDevSubscriberDcs(GetAssDevByDevSubscriberParamModel param)
        {
            var criteria = new GetAssDevByDevSubscriberDcsCriteria
            {
                DeviceID = param.DeviceID,
                NoOfRecord = param.NoOfRecord,
                SubscriberID = param.SubscriberID,
            };

            return repository.Fetch<GetAssDevByDevSubscriberItemModel>(criteria);
        }
        #endregion

        #region report
        public TransactionResultModel GetTransactionHistoryDcs(TransactionParamModel param)
        {
            if (param.CTSCustID != null && param.CTSCustID > 0)
            {
                return GetTransactionByCustSubscriberDcs(param);
            }

            return GetTransactionBySubscriberDcs(param);
        }

        private TransactionResultModel GetTransactionByCustSubscriberDcs(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryByCustSubscriberDcsCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<TransactionDcsItemModel>(criteria).Select(x => new TransactionItemModel()
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

        private TransactionResultModel GetTransactionBySubscriberDcs(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryBySubscriberDcsCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                SubscriberID = param.SubscriberID,
                UserName = param.UserName,
                IsPartial = param.IsPartial,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<TransactionDcsItemModel>(criteria).Select(x => new TransactionItemModel()
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

        public LastestTransactionResultModel GetLastestTransactionBySubscriberDcs(LastestTransactionParamModel param)
        {
            var data = new LastestTransactionResultModel();

            var criteria = new GetLastestTransactionDcsCriteria
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
