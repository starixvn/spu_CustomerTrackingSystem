namespace Fanex.NAP.CTS.Criteria.AssociationReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetSubscriberGroupCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsTest")]
        public bool IsTest { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Subscriber_GetByGroup";

        public override bool IsValid()
            => true;
    }
}
