namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MatchMonitorDetailVerifyCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_MatchID", AllowNull = true)]
        public Int64 MatchID { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator", AllowNull = true)]
        public bool LiveIndicator { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID", AllowNull = true)]
        public int BettypeID { get; set; }

        [SpParam(SqlParamName = "ip_BetID", AllowNull = true)]
        public int? BetID { get; set; }

        [SpParam(SqlParamName = "ip_ListTransID", AllowNull = true)]
        public string TranIds { get; set; }

        [SpParam(SqlParamName = "ip_VerifiedBy", AllowNull = true)]
        public int VerifiedBy { get; set; }

        [SpParam(SqlParamName = "ip_TicketType", AllowNull = true)]
        public int TicketType { get; set; } // 1: Single, 2: Parlay

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_Details_Verify";

        public override bool IsValid()
            => true;
    }
}
