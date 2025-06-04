namespace Fanex.NAP.CTS.Criteria.Account.RobotSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetRDTransCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_PeriodRangeType", AllowNull = true)]
        public long PeriodRangeType { get; set; }

        [SpParam(SqlParamName = "ip_CustID", AllowNull = true)]
        public long CustID { get; set; }

        [SpParam(SqlParamName = "ip_MatchID", AllowNull = true)]
        public long MatchID { get; set; }     

        public override string GetSettingKey()
        {
            return "SPU_AIML_RD_GetRobotUserMatchTransList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
