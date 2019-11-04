namespace Fanex.NAP.CTS.Library.Services
{
    using Fanex.NAP.CTS.Library.Models.Subscriber;
    using Fanex.NAP.CTS.Library.ViewModels.Account;
    using Fanex.NAP.CTS.Library.ViewModels.Account.Associated;
    using Fanex.NAP.CTS.Library.ViewModels.Account.GeneralInformation;
    using Fanex.NAP.CTS.Library.ViewModels.Account.Search;
    using System.Collections.Generic;

    public interface IAccountService
    {
        IEnumerable<SubscriberAssignment> GetSubscribers(long userId);

        IEnumerable<SubscriberAccount> SearchAccounts(SearchAccountFilter filter);

        AccountDetail GetAccountDetailGeneralInformation(GeneralInformationFilter filter);

        IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccounts(AssociatedAccountByAccountFilter filter, out string msgError);

        string GetShortestPath(int startCustId, int endCustId);

        IEnumerable<AssociatedDeviceByAccount> GetAssociatedDevicesByAccount(AssociatedDeviceByAccountFilter filter);        
    }
}