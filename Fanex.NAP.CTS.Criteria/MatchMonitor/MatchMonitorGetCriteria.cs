namespace Fanex.NAP.CTS.Criteria.ManageSubscriber
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MatchMonitorGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime ip_FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ip_ToDate { get; set; }

        [SpParam(SqlParamName = "ip_Bettypes")]
        public string ip_BetType { get; set; }

        [SpParam(SqlParamName = "ip_IsVerified")]
        public int? ip_IsVerified { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_Get";

        public override bool IsValid()
            => true;
    }
}
