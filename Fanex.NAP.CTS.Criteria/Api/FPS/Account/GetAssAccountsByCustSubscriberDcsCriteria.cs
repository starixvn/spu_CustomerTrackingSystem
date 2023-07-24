namespace Fanex.NAP.CTS.Criteria.Api.FPS.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssAccountsByCustSubscriberDcsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_AccountID")]
        public long CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_NoOfRecord")]
        public int NoOfRecord { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_Association_GetAssociationListByAccountSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
