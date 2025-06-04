namespace Fanex.NAP.CTS.Criteria.Account.RobotSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetRobotDetectionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID", AllowNull = true)]
        public long CustID { get; set; }       

        [SpParam(SqlParamName = "op_FirstRobotDate", AllowNull = true)]
        public DateTime? FirstRobotDate { get; set; }

        [SpParam(SqlParamName = "op_LastRobotDate", AllowNull = true)]
        public DateTime? LastRobotDate { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_RobotDetection_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
