namespace Fanex.NAP.CTS.Criteria.MatchMonitor.Parlay
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    public class MMParlaySuspiciousTicketGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_MatchID", AllowNull = true)]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator", AllowNull = true)]
        public bool LiveIndicator { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID", AllowNull = true)]
        public int BettypeID { get; set; }

        [SpParam(SqlParamName = "ip_BetID", AllowNull = true)]
        public long BetID { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitorParlay_Details_Get";

        public override bool IsValid()
            => true;
    }
}
