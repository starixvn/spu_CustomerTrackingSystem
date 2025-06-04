namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data.Repository;

    public class SubscriberGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CTS_DC_Subscriber_GetAll";

        public override bool IsValid()
            => true;
    }
}
