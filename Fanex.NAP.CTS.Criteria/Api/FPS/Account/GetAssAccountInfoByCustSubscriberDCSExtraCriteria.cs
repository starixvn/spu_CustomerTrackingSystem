namespace Fanex.NAP.CTS.Criteria.Api.FPS.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssAccountInfoByCustSubscriberDCSExtraCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_AccountID")]
        public long CTSCustID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_Association_GetAccountInfoByAccountSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
