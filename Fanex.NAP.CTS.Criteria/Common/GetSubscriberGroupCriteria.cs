namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetSubscriberGroupCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CTS_DC_Subscriber_GetByGroup";

        public override bool IsValid()
            => true;
    }
}
