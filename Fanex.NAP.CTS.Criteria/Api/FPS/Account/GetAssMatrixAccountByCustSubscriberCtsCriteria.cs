namespace Fanex.NAP.CTS.Criteria.Api.FPS.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssMatrixAccountByCustSubscriberCtsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_NoOfCustomer")]
        public int NoOfCustomer { get; set; }

        [SpParam(SqlParamName = "ip_NoOfDevice")]
        public int NoOfDevice { get; set; }

        public override string GetSettingKey() => "CTS_DC_Association_GetAssociationMatrixByCustSubscriber";

        public override bool IsValid() => true;
    }
}
