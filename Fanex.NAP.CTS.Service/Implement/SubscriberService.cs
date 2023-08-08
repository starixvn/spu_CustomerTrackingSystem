using Fanex.NAP.CTS.Core.BaseClasses;
using Fanex.NAP.CTS.Criteria.Api;
using Fanex.NAP.CTS.Criteria.Api.FPS.Subscriber;
using Fanex.NAP.CTS.Model;
using Fanex.NAP.CTS.Model.API.FPS.Subscriber;
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
                SubscriberName = subscriberName
            };

            return DataService.GetSingle<SubscriberInfo>(criteria);
        }

        public SubscriberFpsInfo GetSubscriberDcsExBySubscriberName(string subscriberName)
        {
            var criteria = new GetSubscriberDcsExBySubscriberName
            {
                SubscriberName = subscriberName
            };

            return DataService.GetSingle<SubscriberFpsInfo>(criteria);
        }
    }
}
