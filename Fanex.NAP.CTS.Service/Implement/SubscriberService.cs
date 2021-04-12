using Fanex.NAP.CTS.Core.BaseClasses;
using Fanex.NAP.CTS.Criteria.Api;
using Fanex.NAP.CTS.Model.Subscriber;
using Fanex.NAP.CTS.Service.Interface;

namespace Fanex.NAP.CTS.Service.Implement
{
    public class SubscriberService : ISubscriberService
    {
        public SubscriberInfo GetSubscriberBySubscriberName(string subscriberName)
        {
            var criteria = new GetSubscriberInfoBySubscriberName
            {
               SubscriberName  = subscriberName
            };

            return DataService.GetSingle<SubscriberInfo>(criteria);
        }
    }
}
