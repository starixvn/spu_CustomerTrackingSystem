namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SubscriberUpdateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberName", AllowNull = true)]
        public string SubscriberName { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberType", AllowNull = true)]
        public int SubscriberType { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberSourceID", AllowNull = true)]
        public int SubscriberSourceID { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberPrefix", AllowNull = true)]
        public string SubscriberPrefix { get; set; }

        [SpParam(SqlParamName = "ip_IsTest", AllowNull = true)]
        public int IsTest { get; set; }

        [SpParam(SqlParamName = "ip_SiteList", AllowNull = true)]
        public string SiteList { get; set; }

        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public long UserID { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Subscriber_Update";

        public override bool IsValid()
            => true;
    }
}
