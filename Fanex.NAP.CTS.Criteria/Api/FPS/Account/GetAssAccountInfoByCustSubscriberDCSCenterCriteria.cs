namespace Fanex.NAP.CTS.Criteria.Api.FPS.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssAccountInfoByCustSubscriberDCSCenterCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_GetAccountInfoByCustSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
