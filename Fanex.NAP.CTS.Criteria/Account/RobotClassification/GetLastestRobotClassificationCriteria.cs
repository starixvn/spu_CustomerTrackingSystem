namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetLastestRobotClassificationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public long CustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_RobotClassification_GetLastest";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}
