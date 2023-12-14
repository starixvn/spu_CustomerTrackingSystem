namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.ViewModel.Common;
    using System.Collections.Generic;

    public interface ICommonService
    {
        IEnumerable<SubscriberGroupViewModel> GetSubscriberGroups();

        IEnumerable<SubscriberGroupModel> GetSubscribers();

        IEnumerable<ValidateUserNameModel> ValidateUsernames(string userNames, out bool isException);

        IEnumerable<SiteModel> GetSites(bool isNestedCalled);

        IEnumerable<MainMatchModel> GetMainMatchInfosByIDs(string matchIDs);

        IEnumerable<MainTicketDetailModel> GetMainTicketDetails(string transIds, int? transSize, int? matchSize);

        IEnumerable<CCGroupModel> GetGroupCustomerClass();

        int GetBetID(string betIDPattern, string nameDetailsX);

        IEnumerable<StaticListModel> GetStaticLists(int listID, bool isStatusActive);

        IEnumerable<BetTypeSettingModel> GetBetTypeSettings(BetTypeSettingParaModel infos);
        
    }
}
