namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MatchMonitorGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_LeagueName")]
        public string LeagueName { get; set; }

        [SpParam(SqlParamName = "ip_Market")]
        public int? Market { get; set; }

        [SpParam(SqlParamName = "ip_MatchSection")]
        public string MatchSection { get; set; }

        [SpParam(SqlParamName = "ip_IsVerified")]
        public int? IsVerified { get; set; }

        [SpParam(SqlParamName = "ip_Bettypes")]
        public string BetType { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_Get";

        public override bool IsValid()
            => true;
    }
}