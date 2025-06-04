namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetHistoryRobotClassificationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID", AllowNull = true)]
        public long CustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_RobotClassification_GetHistory";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}
