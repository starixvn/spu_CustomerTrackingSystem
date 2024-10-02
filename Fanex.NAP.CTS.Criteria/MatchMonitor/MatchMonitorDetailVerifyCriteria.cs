namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MatchMonitorDetailVerifyCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_MatchID")]
        public Int64 MatchID { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator")]
        public bool LiveIndicator { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID")]
        public int BettypeID { get; set; }

        [SpParam(SqlParamName = "ip_BetID")]
        public int? BetID { get; set; }

        [SpParam(SqlParamName = "ip_ListTransID")]
        public string TranIds { get; set; }

        [SpParam(SqlParamName = "ip_VerifiedBy")]
        public int VerifiedBy { get; set; }

        [SpParam(SqlParamName = "ip_TicketType")]
        public int TicketType { get; set; } // 1: Single, 2: Parlay

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_Details_Verify";

        public override bool IsValid()
            => true;
    }
}
