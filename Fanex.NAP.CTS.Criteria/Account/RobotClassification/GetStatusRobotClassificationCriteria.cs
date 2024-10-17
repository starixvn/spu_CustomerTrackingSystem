namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class GetStatusRobotClassificationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public long CustID { get; set; }

        [SpParam(Ignored = true)]
        public ROLE_CUSTOMER RoleID { get; set; }

        public override string GetSettingKey()
        {
            return this.RoleID == ROLE_CUSTOMER.MEMBER
                ? "CTS_DC_RobotClassification_GetRobotStatus"
                : "CTS_DC_RobotClassificationAgency_GetRobotStatus";
        }       

        public override bool IsValid()
        {
            return CustID > 0;
        }
    }
}
