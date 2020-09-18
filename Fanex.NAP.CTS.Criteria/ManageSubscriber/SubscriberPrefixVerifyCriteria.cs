namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SubscriberPrefixVerifyCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberPrefix")]
        public string SubscriberPrefix { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Subscriber_ValidatePrefix";

        public override bool IsValid()
            => true;
    }
}
