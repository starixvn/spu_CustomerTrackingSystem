namespace Fanex.NAP.CTS.Criteria.Account.RobotSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetRobotUsersSCEDetailsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID", AllowNull = true)]
        public long CustID { get; set; }

        [SpParam(SqlParamName = "ip_RobotTypeOrigin", AllowNull = true)]
        public int RobotTypeOrigin { get; set; }

        [SpParam(SqlParamName = "ip_RobotTypeDisplay", AllowNull = true)]
        public int RobotTypeDisplay { get; set; }

        [SpParam(SqlParamName = "ip_ScannedDate", AllowNull = true)]
        public DateTime? ScannedDate { get; set; }

        [SpParam(SqlParamName = "ip_QueryType", AllowNull = true)]
        public int QueryType { get; set; }

        [SpParam(SqlParamName = "ip_ClusterID", AllowNull = true)]
        public int? ClusterID { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_RD_SCE_GetRobotUsersDetails";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
