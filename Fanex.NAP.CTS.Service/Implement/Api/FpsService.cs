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
    using Newtonsoft.Json;
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
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssAccountsByCustSubscriberItemModel>(criteria);

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
        public TransactionResultModel GetTransactionHistoryDcs(TransactionParamModel param)
        {
            //if (param.CTSCustID != null && param.CTSCustID > 0)
            //{
            //    return GetTransactionByCustSubscriberDcs(param);
            //}

            //return GetTransactionBySubscriberDcs(param);

            var jsonString = "{\"Items\":[{\"TransTime\":\"2023-06-28T02:50:47.8475\",\"TransID\":4892831288,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"c0aa50ab6ccd49a1b3234e2e955eefcb\",\"Action\":null,\"ActionResult\":null,\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":\"http://portuguese.french_toast146.326.comcanh\",\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-28T01:36:14.5725\",\"TransID\":4935938354,\"CTSCustID\":20171,\"SubscriberID\":6,\"RegisterName\":\"4321yyy\",\"UserName\":\"12BETRMB0144540\",\"FirstDeviceCode\":\"fa365d2a2e304f94b30500844e9a88a3\",\"Action\":\"login\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":\"http://alpha4.nexdev.net\",\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-27T01:26:55.117\",\"TransID\":4892831338,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"7056e33be7db45719a396c36db9405ad\",\"Action\":null,\"ActionResult\":null,\"OS\":\"Windows\",\"Browser\":\"Chrome\",\"URLDetails\":\"http://portuguese.french_toast146.326.comcanh\",\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Invalid Browser Info\"},{\"TransTime\":\"2023-06-26T23:16:55.831\",\"TransID\":4935938351,\"CTSCustID\":20171,\"SubscriberID\":6,\"RegisterName\":\"4321yyy\",\"UserName\":\"12BETRMB0144540\",\"FirstDeviceCode\":\"fa365d2a2e304f94b30500844e9a88a3\",\"Action\":\"login\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":\"http://alpha4.nexdev.net\",\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-26T23:00:00\",\"TransID\":4892831287,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"c0aa50ab6ccd49a1b3234e2e955eefcb\",\"Action\":null,\"ActionResult\":null,\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":\"http://portuguese.french_toast146.326.comcanh\",\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-12T23:59:47.8475\",\"TransID\":4892831286,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"cf5517b3eb3a4748bf2efd187df2ae97\",\"Action\":null,\"ActionResult\":null,\"OS\":\"Windows\",\"Browser\":\"Chrome\",\"URLDetails\":\"http://portuguese.french_toast146.326.comcanh\",\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-12T04:42:53.0385\",\"TransID\":4935938634,\"CTSCustID\":20171,\"SubscriberID\":6,\"RegisterName\":\"4321yyy\",\"UserName\":\"12BETRMB0144540\",\"FirstDeviceCode\":\"d4eda824f46742de9769c5ccd783283e\",\"Action\":\"login\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Edge\",\"URLDetails\":\"http://alpha4.nexdev.net\",\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-12T04:39:33.9047\",\"TransID\":4935938632,\"CTSCustID\":20171,\"SubscriberID\":6,\"RegisterName\":\"4321yyy\",\"UserName\":\"12BETRMB0144540\",\"FirstDeviceCode\":\"d4eda824f46742de9769c5ccd783283e\",\"Action\":\"login\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Edge\",\"URLDetails\":\"http://alpha4.nexdev.net\",\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-09T00:37:27.8273\",\"TransID\":4935938379,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":null,\"Action\":null,\"ActionResult\":null,\"OS\":null,\"Browser\":null,\"URLDetails\":null,\"IP\":null,\"RobotTracking\":null},{\"TransTime\":\"2023-06-08T23:54:15.4503\",\"TransID\":4935938377,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"8a0ca07999c447c388a7d503d0362a96\",\"Action\":\"login\",\"ActionResult\":\"successfully\",\"OS\":\"Mac OS X\",\"Browser\":\"Safari\",\"URLDetails\":\"http://alpha20.nexdev.net\",\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Bot\"},{\"TransTime\":\"2023-06-08T22:57:23.5891\",\"TransID\":4935938367,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"698d19907c4d41ac8d024429883287aa\",\"Action\":\"login\",\"ActionResult\":\"successfully\",\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":\"http://alpha20.nexdev.net\",\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Bot\"},{\"TransTime\":\"2023-06-08T22:44:08.9026\",\"TransID\":4935938362,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"cf5517b3eb3a4748bf2efd187df2ae97\",\"Action\":\"login\",\"ActionResult\":\"successfully\",\"OS\":\"Windows\",\"Browser\":\"Chrome\",\"URLDetails\":\"http://alpha20.nexdev.net\",\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Bot\"},{\"TransTime\":\"2023-06-08T05:11:29.5285\",\"TransID\":4935938349,\"CTSCustID\":20171,\"SubscriberID\":6,\"RegisterName\":\"4321yyy\",\"UserName\":\"12BETRMB0144540\",\"FirstDeviceCode\":\"fa365d2a2e304f94b30500844e9a88a3\",\"Action\":\"login\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":\"http://alpha4.nexdev.net\",\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:30:18.2912\",\"TransID\":4935937588,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"a2c70b4cb77340edbb362e387625de64\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Android\",\"Browser\":\"Chrome Mobile WebView\",\"URLDetails\":\"http://chinese.yangzhou_fried_rice6.4521656.comcanh\",\"IP\":\"136.178.137.135\",\"RobotTracking\":\"Bot\"},{\"TransTime\":\"2023-06-05T04:30:08.9501\",\"TransID\":4935937270,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"8646ec58a73d477b9c5f562e25818af8\",\"Action\":\"deposit\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Chrome\",\"URLDetails\":\"http://norwegian.meatballs6.29261122.comcanh\",\"IP\":\"103.128.150.106\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:30:08.0771\",\"TransID\":4935937359,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"ec6b4a8c6b1c4991996f3c4e55207924\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"iOS\",\"Browser\":\"Chrome Mobile iOS\",\"URLDetails\":\"http://portuguese.caldo_verde6.30344509.comcanh\",\"IP\":\"152.145.170.152\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:30:02.5093\",\"TransID\":4935936941,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"04075c607bca45c78a2cf9c7cee1b5fa\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"iOS\",\"Browser\":\"Mobile Safari\",\"URLDetails\":\"http://indian.butter_chicken6.8366.comcanh\",\"IP\":\"123.103.125.137\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:29:47.8903\",\"TransID\":4935936810,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"dba3936e4c174c96b576a3ce895d7478\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Android\",\"Browser\":\"Chrome Mobile WebView\",\"URLDetails\":\"http://chinese.yangzhou_fried_rice6.4521656.comcanh\",\"IP\":\"146.146.111.179\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:29:25.5266\",\"TransID\":4935936590,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":null,\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Android\",\"Browser\":\"Chrome\",\"URLDetails\":\"http://italian.margherita_pizza6.8891.comcanh\",\"IP\":\"169.176.110.167\",\"RobotTracking\":\"Invalid Browser Info\"},{\"TransTime\":\"2023-06-05T04:29:22.4925\",\"TransID\":4935936524,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":null,\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"iOS\",\"Browser\":\"Mobile Safari\",\"URLDetails\":\"http://indian.butter_chicken6.8366.comcanh\",\"IP\":\"169.125.100.107\",\"RobotTracking\":\"Invalid Browser Info\"},{\"TransTime\":\"2023-06-05T04:29:22.21\",\"TransID\":4935936533,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"96c82b8c5ef849d78430a25a44e1a3e0\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Android\",\"Browser\":\"Chrome Mobile\",\"URLDetails\":\"http://indian.butter_chicken6.8366.comcanh\",\"IP\":\"134.168.179.128\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:28:21.829\",\"TransID\":4935935977,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"e08ad8a3dcbb4251886c79e6c5da0288\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"iOS\",\"Browser\":\"Mobile Safari\",\"URLDetails\":\"http://chinese.yangzhou_fried_rice6.8418.comcanh\",\"IP\":\"175.136.139.104\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:28:14.6486\",\"TransID\":4935935956,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"b6b39360e6d54b278d04ce80dc75ef3b\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"iOS\",\"Browser\":\"Mobile Safari\",\"URLDetails\":\"http://indian.butter_chicken6.8366.comcanh\",\"IP\":\"161.145.143.101\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:28:09.9021\",\"TransID\":4935935815,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"0ad3ed0ac315484d9678fa929abc2b37\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Windows\",\"Browser\":\"Chrome\",\"URLDetails\":\"http://russian.pirog6.8740.comcanh\",\"IP\":\"159.160.144.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:28:03.4209\",\"TransID\":4935935755,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"d0aa20a833d74da0bdd6d0178338ea08\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Android\",\"Browser\":\"Chrome Mobile WebView\",\"URLDetails\":\"http://chinese.yangzhou_fried_rice6.4521656.comcanh\",\"IP\":\"151.166.113.129\",\"RobotTracking\":\"Human\"}],\"TotalItem\":2755,\"IsTimeout\":false}\r\n";
            var data = JsonConvert.DeserializeObject<TransactionResultModel>(jsonString);

            return data;
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

            data.Items = repository.Fetch<TransactionItemModel>(criteria);
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

            data.Items = repository.Fetch<TransactionItemModel>(criteria);
            data.TotalItem = criteria.TotalItem;

            return data;
        }

        public LastestTransactionResultModel GetLastestTransactionBySubscriberDcs(LastestTransactionParamModel param)
        {
            var jsonString = "{\"Items\":[{\"TransTime\":\"2023-06-28T02:50:47.8475\",\"TransID\":4892831288,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"c0aa50ab6ccd49a1b3234e2e955eefcb\",\"Action\":null,\"ActionResult\":null,\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":null,\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-28T01:36:14.5725\",\"TransID\":4935938354,\"CTSCustID\":20171,\"SubscriberID\":6,\"RegisterName\":\"4321yyy\",\"UserName\":\"12BETRMB0144540\",\"FirstDeviceCode\":\"fa365d2a2e304f94b30500844e9a88a3\",\"Action\":\"login\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":null,\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-27T01:26:55.117\",\"TransID\":4892831338,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"7056e33be7db45719a396c36db9405ad\",\"Action\":null,\"ActionResult\":null,\"OS\":\"Windows\",\"Browser\":\"Chrome\",\"URLDetails\":null,\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Invalid Browser Info\"},{\"TransTime\":\"2023-06-26T23:16:55.831\",\"TransID\":4935938351,\"CTSCustID\":20171,\"SubscriberID\":6,\"RegisterName\":\"4321yyy\",\"UserName\":\"12BETRMB0144540\",\"FirstDeviceCode\":\"fa365d2a2e304f94b30500844e9a88a3\",\"Action\":\"login\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":null,\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-26T23:00:00\",\"TransID\":4892831287,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"c0aa50ab6ccd49a1b3234e2e955eefcb\",\"Action\":null,\"ActionResult\":null,\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":null,\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-12T23:59:47.8475\",\"TransID\":4892831286,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"cf5517b3eb3a4748bf2efd187df2ae97\",\"Action\":null,\"ActionResult\":null,\"OS\":\"Windows\",\"Browser\":\"Chrome\",\"URLDetails\":null,\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-12T04:42:53.0385\",\"TransID\":4935938634,\"CTSCustID\":20171,\"SubscriberID\":6,\"RegisterName\":\"4321yyy\",\"UserName\":\"12BETRMB0144540\",\"FirstDeviceCode\":\"d4eda824f46742de9769c5ccd783283e\",\"Action\":\"login\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Edge\",\"URLDetails\":null,\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-12T04:39:33.9047\",\"TransID\":4935938632,\"CTSCustID\":20171,\"SubscriberID\":6,\"RegisterName\":\"4321yyy\",\"UserName\":\"12BETRMB0144540\",\"FirstDeviceCode\":\"d4eda824f46742de9769c5ccd783283e\",\"Action\":\"login\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Edge\",\"URLDetails\":null,\"IP\":\"10.0.0.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-09T00:37:27.8273\",\"TransID\":4935938379,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":null,\"Action\":null,\"ActionResult\":null,\"OS\":null,\"Browser\":null,\"URLDetails\":null,\"IP\":null,\"RobotTracking\":null},{\"TransTime\":\"2023-06-08T23:54:15.4503\",\"TransID\":4935938377,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"8a0ca07999c447c388a7d503d0362a96\",\"Action\":\"login\",\"ActionResult\":\"successfully\",\"OS\":\"Mac OS X\",\"Browser\":\"Safari\",\"URLDetails\":null,\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Bot\"},{\"TransTime\":\"2023-06-08T22:57:23.5891\",\"TransID\":4935938367,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"698d19907c4d41ac8d024429883287aa\",\"Action\":\"login\",\"ActionResult\":\"successfully\",\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":null,\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Bot\"},{\"TransTime\":\"2023-06-08T22:44:08.9026\",\"TransID\":4935938362,\"CTSCustID\":15187,\"SubscriberID\":6,\"RegisterName\":\"hetteteoi\",\"UserName\":\"12BETINH0134558\",\"FirstDeviceCode\":\"cf5517b3eb3a4748bf2efd187df2ae97\",\"Action\":\"login\",\"ActionResult\":\"successfully\",\"OS\":\"Windows\",\"Browser\":\"Chrome\",\"URLDetails\":null,\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Bot\"},{\"TransTime\":\"2023-06-08T05:11:29.5285\",\"TransID\":4935938349,\"CTSCustID\":20171,\"SubscriberID\":6,\"RegisterName\":\"4321yyy\",\"UserName\":\"12BETRMB0144540\",\"FirstDeviceCode\":\"fa365d2a2e304f94b30500844e9a88a3\",\"Action\":\"login\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Firefox\",\"URLDetails\":null,\"IP\":\"10.0.0.116\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:30:18.2912\",\"TransID\":4935937588,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"a2c70b4cb77340edbb362e387625de64\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Android\",\"Browser\":\"Chrome Mobile WebView\",\"URLDetails\":\"http://chinese.yangzhou_fried_rice6.4521656.comcanh\",\"IP\":\"136.178.137.135\",\"RobotTracking\":\"Bot\"},{\"TransTime\":\"2023-06-05T04:30:08.9501\",\"TransID\":4935937270,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"8646ec58a73d477b9c5f562e25818af8\",\"Action\":\"deposit\",\"ActionResult\":\"\",\"OS\":\"Windows\",\"Browser\":\"Chrome\",\"URLDetails\":\"http://norwegian.meatballs6.29261122.comcanh\",\"IP\":\"103.128.150.106\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:30:08.0771\",\"TransID\":4935937359,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"ec6b4a8c6b1c4991996f3c4e55207924\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"iOS\",\"Browser\":\"Chrome Mobile iOS\",\"URLDetails\":\"http://portuguese.caldo_verde6.30344509.comcanh\",\"IP\":\"152.145.170.152\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:30:02.5093\",\"TransID\":4935936941,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"04075c607bca45c78a2cf9c7cee1b5fa\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"iOS\",\"Browser\":\"Mobile Safari\",\"URLDetails\":\"http://indian.butter_chicken6.8366.comcanh\",\"IP\":\"123.103.125.137\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:29:47.8903\",\"TransID\":4935936810,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"dba3936e4c174c96b576a3ce895d7478\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Android\",\"Browser\":\"Chrome Mobile WebView\",\"URLDetails\":\"http://chinese.yangzhou_fried_rice6.4521656.comcanh\",\"IP\":\"146.146.111.179\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:29:25.5266\",\"TransID\":4935936590,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":null,\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Android\",\"Browser\":\"Chrome\",\"URLDetails\":\"http://italian.margherita_pizza6.8891.comcanh\",\"IP\":\"169.176.110.167\",\"RobotTracking\":\"Invalid Browser Info\"},{\"TransTime\":\"2023-06-05T04:29:22.4925\",\"TransID\":4935936524,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":null,\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"iOS\",\"Browser\":\"Mobile Safari\",\"URLDetails\":\"http://indian.butter_chicken6.8366.comcanh\",\"IP\":\"169.125.100.107\",\"RobotTracking\":\"Invalid Browser Info\"},{\"TransTime\":\"2023-06-05T04:29:22.21\",\"TransID\":4935936533,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"96c82b8c5ef849d78430a25a44e1a3e0\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Android\",\"Browser\":\"Chrome Mobile\",\"URLDetails\":\"http://indian.butter_chicken6.8366.comcanh\",\"IP\":\"134.168.179.128\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:28:21.829\",\"TransID\":4935935977,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"e08ad8a3dcbb4251886c79e6c5da0288\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"iOS\",\"Browser\":\"Mobile Safari\",\"URLDetails\":\"http://chinese.yangzhou_fried_rice6.8418.comcanh\",\"IP\":\"175.136.139.104\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:28:14.6486\",\"TransID\":4935935956,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"b6b39360e6d54b278d04ce80dc75ef3b\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"iOS\",\"Browser\":\"Mobile Safari\",\"URLDetails\":\"http://indian.butter_chicken6.8366.comcanh\",\"IP\":\"161.145.143.101\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:28:09.9021\",\"TransID\":4935935815,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"0ad3ed0ac315484d9678fa929abc2b37\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Windows\",\"Browser\":\"Chrome\",\"URLDetails\":\"http://russian.pirog6.8740.comcanh\",\"IP\":\"159.160.144.138\",\"RobotTracking\":\"Human\"},{\"TransTime\":\"2023-06-05T04:28:03.4209\",\"TransID\":4935935755,\"CTSCustID\":12837,\"SubscriberID\":6,\"RegisterName\":\"pellie\",\"UserName\":\"12BETGBP013503\",\"FirstDeviceCode\":\"d0aa20a833d74da0bdd6d0178338ea08\",\"Action\":\"login\",\"ActionResult\":\"success\",\"OS\":\"Android\",\"Browser\":\"Chrome Mobile WebView\",\"URLDetails\":\"http://chinese.yangzhou_fried_rice6.4521656.comcanh\",\"IP\":\"151.166.113.129\",\"RobotTracking\":\"Human\"}],\"TotalItem\":100}";
            var data = JsonConvert.DeserializeObject<LastestTransactionResultModel>(jsonString);
            //var data = new LastestTransactionResultModel();

            //var criteria = new GetLastestTransactionDcsCriteria
            //{
            //    SubscriberID = param.SubscriberID,
            //    Length = param.Length,
            //    Skip = param.Skip,
            //    Take = param.Take
            //};

            //data.Items = repository.Fetch<LastestTransactionItemModel>(criteria);
            //data.TotalItem = criteria.TotalItem;

            return data;
        }
        #endregion dcs
    }
}
