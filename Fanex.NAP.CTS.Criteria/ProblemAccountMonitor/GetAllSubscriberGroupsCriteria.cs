namespace Fanex.NAP.CTS.Criteria.ProblemAccountMonitor
{
    using Fanex.Data.Repository;

    public class GetAllSubscriberGroupsCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CTS_DC_GetAllSubscriberGroups";

        public override bool IsValid()
            => true;
    }
}