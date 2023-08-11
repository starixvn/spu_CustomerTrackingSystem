namespace Fanex.NAP.CTS.Criteria.Api.FPS.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssMatrixAccountByCustSubscriberDCSExtraCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_AccountID")]
        public long CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_NoOfCustomer")]
        public int NoOfCustomer { get; set; }

        [SpParam(SqlParamName = "ip_NoOfDevice")]
        public int NoOfDevice { get; set; }

        public override string GetSettingKey() => "DCS_ET_Association_GetAssociationMatrixByAccountSubscriber";

        public override bool IsValid() => true;
    }
}
