namespace Fanex.NAP.CTS.Criteria.Api.FPS.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssMatrixAccountByCustSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_Skip")]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "ip_Take")]
        public int Take { get; set; }

        [SpParam(SqlParamName = "op_TotalPages")]
        public int TotalPages { get; set; }

        [SpParam(SqlParamName = "op_TotalRows")]
        public int TotalRows { get; set; }

        public override string GetSettingKey() => "CTS_DC_Association_GetAssociationMaxtrixByCustSubscriber";

        public override bool IsValid() => true;
    }
}
