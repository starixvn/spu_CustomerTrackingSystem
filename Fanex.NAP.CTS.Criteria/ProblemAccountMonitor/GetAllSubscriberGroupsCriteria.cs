namespace Fanex.NAP.CTS.Criteria.ProblemAccountMonitor
{
    using Fanex.Data.Repository;

    public class GetAllSubscriberGroupsCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CTS_DC_Common_GetAllSubscriberGroups";

        public override bool IsValid()
            => true;
    }
}