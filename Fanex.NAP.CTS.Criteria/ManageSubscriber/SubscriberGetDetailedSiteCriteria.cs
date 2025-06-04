namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SubscriberGetDetailedSiteCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public long ip_SubscriberID { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Subscriber_GetDetails";

        public override bool IsValid()
            => true;
    }
}
