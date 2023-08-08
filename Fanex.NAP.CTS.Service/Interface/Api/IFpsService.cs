namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API.FPS.Account;
    using Fanex.NAP.CTS.Model.API.FPS.Device;
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using Fanex.NAP.CTS.Model.API.FPS.Transaction;

    public interface IFpsService
    {
        #region cts
        SearchAccountResultModel SearchAccountCts(SearchAccountParamModel param);

        GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriberCts(GetAssAccountInfoByCustSubscriberParamModel param);

        TransactionResultModel GetTransactionHistoryCts(TransactionParamModel param);

        GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriberCts(GetAssAccountsByCustSubscriberParamModel param);

        GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriberCts(GetAssDevicesByCustSubscriberParamModel param);

        GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriberCts(GetAssMatrixAccountByCustSubscriberParamModel param);

        LastestTransactionResultModel GetLastestTransactionBySubscriberCts(LastestTransactionParamModel param);
        #endregion cts

        #region dcs
        SearchAccountResultModel SearchAccountDcs(SearchAccountParamModel param);

        GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriberDcs(GetAssAccountInfoByCustSubscriberParamModel param);


        TransactionResultModel GetTransactionHistoryDcs(TransactionParamModel param);

        LastestTransactionResultModel GetLastestTransactionBySubscriberDcs(LastestTransactionParamModel param);

        GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriberDcs(GetAssAccountsByCustSubscriberParamModel param);

        GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriberDcs(GetAssDevicesByCustSubscriberParamModel param);

        GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriberDcs(GetAssMatrixAccountByCustSubscriberParamModel param);
        #endregion dcs
    }
}
