namespace Fanex.NAP.CTS.Criteria.RobotReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetAIRobotReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsMonthly", AllowNull = true)]
        public bool IsMonthly { get; set; }

        [SpParam(SqlParamName = "ip_IsDaily", AllowNull = true)]
        public bool IsDaily { get; set; }

        [SpParam(SqlParamName = "ip_IsLicensee", AllowNull = true)]
        public bool? IsLicensee { get; set; }

        [SpParam(SqlParamName = "ip_FromDate", AllowNull = true)]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate", AllowNull = true)]
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "CTS_DC_RPT_RobotStatistic_AI";

        public override bool IsValid() => true;
    }
}
