namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.ManageSubscriber;
    using System.Collections.Generic;

    public interface IManageSubscriberService
    {
        List<SubscriberModel> GetSubscribers();

        List<DetailedSiteModel> GetDetailedSite(long subscriberID);

        DBResult RemoveSubscriber(int subscriberID, long userId);

        UpdateSubsciberResultModel UpdateSubscriber(SubscriberUpdateModel subscriberInsertModel, long userId);

        void ReUpdateSubriberIDBySiteID(List<ReUpdateSubscribeBySiteIDModel> subSite);
    }
}
