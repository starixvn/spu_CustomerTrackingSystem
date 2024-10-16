namespace Fanex.NAP.CTS.Criteria.MatchMonitor.Parlay
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MMParlaySuspiciousTicketGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_MatchID")]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator")]
        public bool LiveIndicator { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID")]
        public int BettypeID { get; set; }

        [SpParam(SqlParamName = "ip_BetID")]
        public long BetID { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitorParlay_Details_Get";

        public override bool IsValid()
            => true;
    }
}
