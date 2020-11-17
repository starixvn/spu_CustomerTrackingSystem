namespace Fanex.NAP.CTS.Criteria.ProbationManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetSubscriberCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CTS_DC_Common_GetAllSubscriberGroups";

        public override bool IsValid()
            => true;
    }
}
