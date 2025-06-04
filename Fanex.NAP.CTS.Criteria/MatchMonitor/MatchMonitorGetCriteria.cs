namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MatchMonitorGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate", AllowNull = true)]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate", AllowNull = true)]
        public DateTime ToDate { get; set; }       

        [SpParam(SqlParamName = "ip_SportType", AllowNull = true)]
        public string SportType { get; set; }

        [SpParam(SqlParamName = "ip_Market", AllowNull = true)]
        public int? Market { get; set; }

        [SpParam(SqlParamName = "ip_IsVerified", AllowNull = true)]
        public int? IsVerified { get; set; }

        [SpParam(SqlParamName = "ip_Bettypes", AllowNull = true)]
        public string BetTypes { get; set; }

        [SpParam(SqlParamName = "ip_Reason", AllowNull = true)]
        public string Reasons { get; set; }   

        [SpParam(SqlParamName = "ip_MatchInfos", AllowNull = true)]
        public string MatchInfosJson { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_Get";

        public override bool IsValid()
            => true;
    }
}
