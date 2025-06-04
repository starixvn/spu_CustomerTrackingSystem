namespace Fanex.NAP.CTS.Criteria.Account.RobotSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;   

    public class GetRDMatchCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_PeriodRangeType", AllowNull = true)]
        public long PeriodRangeType { get; set; }

        [SpParam(SqlParamName = "ip_CustID", AllowNull = true)]
        public long CustID { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_RD_GetRobotUserMatch";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
