
namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.Api;
    using Fanex.NAP.CTS.Criteria.Api.FPS.Subscriber;
    using Fanex.NAP.CTS.Model.API.FPS.Subscriber;
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.Service.Interface;

    public class SubscriberService : ISubscriberService
    {
        private readonly ICriteriaService _repository;

        public SubscriberService()
        {
            _repository = new CriteriaService();
        }

        public SubscriberInfo GetSubscriberBySubscriberName(string subscriberName)
        {
            var criteria = new GetSubscriberInfoBySubscriberName
            {
                SubscriberName = subscriberName
            };

            return _repository.Get<SubscriberInfo>(criteria);
        }

        public SubscriberFpsInfo GetSubscriberBySubscriberName_DCSExtra(string subscriberName)
        {
            var criteria = new GetSubscriberDcsExBySubscriberName
            {
                SubscriberName = subscriberName
            };

            return _repository.Get<SubscriberFpsInfo>(criteria);
        }
    }
}
