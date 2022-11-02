namespace Fanex.NAP.CTS.Criteria.Account.RobotSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetRobotUsersDetailsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public long CustID { get; set; }

        [SpParam(SqlParamName = "ip_PeriodRangeType")]
        public int PeriodRangeType { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_RD_GetRobotUsersDetails";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
