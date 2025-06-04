namespace Fanex.NAP.CTS.Criteria.Account.RobotSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetRobotAiLpDetailCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID", AllowNull = true)]   
        public long CustID { get; set; }

        [SpParam(SqlParamName = "ip_PeriodRangeType", AllowNull = true)]
        public int PeriodRangeType { get; set; }       

        public override string GetSettingKey()
        {
            return "SPU_AIML_RD_LP_GetRobotDetails";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
