using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Api
{
    public class GetSubscriberInfoBySubscriberName : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberName")]
        public string SubscriberName { get; set; }


        public override string GetSettingKey()
        {
            return "CTS_AD_Subscriber_GetInfoBySubscriberName";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
