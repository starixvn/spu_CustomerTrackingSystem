namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API.FPS.Account;
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using Fanex.NAP.CTS.Model.API.FPS.Transaction;

    public interface IFpsService
    {
        SearchAccountResultModel SearchAccount(SearchAccountParamModel param);

        GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriber(GetAssAccountInfoByCustSubscriberParamModel param);

        TransactionResultModel GetTransactionHistory(TransactionParamModel param);

        GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber(GetAssAccountsByCustSubscriberParamModel param);

        LastestTransactionResultModel GetLastestTransactionBySubscriber(LastestTransactionParamModel param);
    }
}
