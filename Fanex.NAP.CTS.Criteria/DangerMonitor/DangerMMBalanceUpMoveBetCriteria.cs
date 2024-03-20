namespace Fanex.NAP.CTS.Criteria.DangerMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class DangerMMBalanceUpMoveBetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime? FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime? ToDate { get; set; }

        public override string GetSettingKey() => "CTS_BalanceUpMoveBet_GetDateRange";

        public override bool IsValid() => true;
    }
}