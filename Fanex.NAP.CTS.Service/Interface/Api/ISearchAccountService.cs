namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using System.Collections.Generic;

    public interface ISearchAccountService
    {
        IEnumerable<SearchAccountResultModel> SearchAccount(int subscriberID, string userNameList);

        IEnumerable<SearchAccountResultModel> SearchPartialAccount(int subscriberID, string userNameList);

    }
}
