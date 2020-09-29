using System;
namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SubscriberUpdateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberName")]
        public string SubscriberName { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberType")]
        public int SubscriberType { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberPrefix")]
        public string SubscriberPrefix { get; set; }

        [SpParam(SqlParamName = "ip_IsTest")]
        public int IsTest { get; set; }

        [SpParam(SqlParamName = "ip_SiteList")]
        public string SiteList { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public long UserID { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Subscriber_Update";

        public override bool IsValid()
            => true;
    }
}
