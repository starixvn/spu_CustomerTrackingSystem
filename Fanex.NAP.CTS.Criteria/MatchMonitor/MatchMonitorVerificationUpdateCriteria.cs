namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MatchMonitorVerificationUpdateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ID")]
        public Int64 ip_ID { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public Int64 ip_UserID { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_UpdateVerify";

        public override bool IsValid()
            => true;
    }
}
