namespace Fanex.NAP.CTS.Criteria.SubscriberAssignment
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAllSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_userId")]
        public int UserId { get; set; }

        public override string GetSettingKey() => "CTS_AD_Subscriber_GetAll";

        public override bool IsValid() => true;

    }
}