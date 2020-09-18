using System;
namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SubscriberUpdateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberList")]
        public string SubscriberList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Subscriber_Update";

        public override bool IsValid()
            => true;
    }
}
