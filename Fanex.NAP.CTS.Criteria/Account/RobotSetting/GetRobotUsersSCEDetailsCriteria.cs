namespace Fanex.NAP.CTS.Criteria.Account.RobotSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetRobotUsersSCEDetailsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public long CustID { get; set; }

        [SpParam(SqlParamName = "ip_RobotType")]
        public int RobotType { get; set; }

        [SpParam(SqlParamName = "ip_ScannedDate")]
        public DateTime? ScannedDate { get; set; }

        [SpParam(SqlParamName = "ip_QueryType")]
        public int QueryType { get; set; }

        [SpParam(SqlParamName = "ip_ClusterID")]
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
