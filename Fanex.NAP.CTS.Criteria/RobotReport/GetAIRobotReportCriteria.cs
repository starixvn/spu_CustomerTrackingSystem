namespace Fanex.NAP.CTS.Criteria.RobotReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetAIRobotReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsMonthly")]
        public bool IsMonthly { get; set; }

        [SpParam(SqlParamName = "ip_IsDaily")]
        public bool IsDaily { get; set; }

        [SpParam(SqlParamName = "ip_IsLicensee")]
        public bool? IsLicensee { get; set; }

        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "CTS_DC_RPT_RobotStatistic_AI";

        public override bool IsValid() => true;
    }
}