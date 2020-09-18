namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SubscriberInsertCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberList")]
        public string SubscriberList { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public long UserID { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Subscriber_Insert";

        public override bool IsValid()
            => true;
    }
}
