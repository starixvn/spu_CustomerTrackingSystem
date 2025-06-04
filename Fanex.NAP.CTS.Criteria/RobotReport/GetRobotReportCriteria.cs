namespace Fanex.NAP.CTS.Criteria.RobotReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetRobotReportCriteria : CriteriaBase
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

        [SpParam(SqlParamName = "ip_CustCounterJs", AllowNull = true)]
        public string CustCounterJs { get; set; }

        public override string GetSettingKey() => "CTS_DC_RPT_RobotStatistic";

        public override bool IsValid() => true;
    }
}
