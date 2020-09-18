namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SubscriberRemoveCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_SiteID")]
        public int SiteID { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Subscriber_Remove";

        public override bool IsValid()
            => true;
    }
}
