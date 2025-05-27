namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API.FPS.Account;
    using Fanex.NAP.CTS.Model.API.FPS.Device;
    using Fanex.NAP.CTS.Model.API.FPS.Search;
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using Fanex.NAP.CTS.Model.API.FPS.Transaction;
    using System.Collections.Generic;

    public interface IFpsService
    {
        #region cts
        SearchAccountResultModel SearchAccount_DCSCenter(SearchAccountParamModel param);

        GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriber_DCSCenter(GetAssAccountInfoByCustSubscriberParamModel param);

        TransactionResultModel GetTransactionHistory_DCSCenter(TransactionParamModel param);

        GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber_DCSCenter(GetAssAccountsByCustSubscriberParamModel param);

        GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriber_DCSCenter(GetAssDevicesByCustSubscriberParamModel param);

        GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriber_DCSCenter(GetAssMatrixAccountByCustSubscriberParamModel param);

        LatestTransactionResultModel GetLastestTransactionBySubscriber_DCSCenter(LatestTransactionParamModel param);

        GetAccountsPerDeviceResultModel GetAccountsPerDevice_DCSCenter(GetAccountPerDeviceParamModel param);

        GetDevicesPerAccountResultModel GetDevicesPerAccount_DCSCenter(GetDevicesPerAccountParamModel param);
        #endregion cts

        #region dcs

        #region search
        SearchAccountResultModel SearchAccount_DCSExtra(SearchAccountParamModel param);

        IEnumerable<SearchDeviceItemDCSExtraModel> SearchDevice_DCSExtra(SearchDeviceParamModel param);
        #endregion

        #region account
        GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriber_DCSExtra(GetAssAccountInfoByCustSubscriberParamModel param);

        GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber_DCSExtra(GetAssAccountsByCustSubscriberParamModel param);

        GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriber_DCSExtra(GetAssDevicesByCustSubscriberParamModel param);

        GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriber_DCSExtra(GetAssMatrixAccountByCustSubscriberParamModel param);
        #endregion

        #region device
        GetAssDeviceInfoByIDSubscriberModel GetAssDeviceInfoByIDSubscriber_DCSExtra(GetAssDeviceInfoByIDSubscriberParamModel param);

        IEnumerable<GetAssAccByDevSubscriberItemModel> GetAssAccByDevSubscriber_DCSExtra(GetAssAccByDevSubscriberParamModel param);

        IEnumerable<GetAssDevByDevSubscriberItemModel> GetAssDevByDevSubscriber_DCSExtra(GetAssDevByDevSubscriberParamModel param);

        IEnumerable<GetAssMatrixByDeviceSubscriberItemModel> GetAssMatrixByDeviceSubscriber_DCSExtra(GetAssMatrixByDeviceSubscriberParamModel param);
        #endregion

        #region report
        TransactionHistoryByDeviceDCSExtraResultModel GetTransactionHistoryByDevice_DCSExtra(TransactionHistoryByDeviceParamModel param);

        TransactionHistoryByAccountDCSExtraResultModel GetTransactionHistoryByAccount_DCSExtra(TransactionHistoryByAccountParamModel param);

        TransactionResultModel GetTransactionHistory_DCSExtra(TransactionParamModel param);

        LatestTransactionResultModel GetLastestTransactionBySubscriber_DCSExtra(LatestTransactionParamModel param);
        #endregion

        #endregion dcs
    }
}
