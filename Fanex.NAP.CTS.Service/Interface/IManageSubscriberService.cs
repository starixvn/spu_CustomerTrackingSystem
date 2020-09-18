namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.ManageSubscriber;
    using System.Collections.Generic;

    public interface IManageSubscriberService
    {
        List<SubscriberModel> GetSubscribers();

        DBResult InsertSubscriber(SubscriberInsertModel subscriberInsertModel, long userId);

        List<ValidateSubscriberPrefixModel> VerifySubscriberToInsert(string prefixSubscriberName);

        DBResult RemoveSubscriber(int subscriberID, int siteID);

        DBResult UpdateSubscriber(SubscriberInsertModel subscriberInsertModel);
    }
}
