namespace Fanex.NAP.CTS.Criteria.Api.FPS.Subscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetSubscriberDcsExBySubscriberName : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberName")]
        public string SubscriberName { get; set; }


        public override string GetSettingKey()
        {
            return "DCS_ET_Subscriber_GetInfoBySubscriberName";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
