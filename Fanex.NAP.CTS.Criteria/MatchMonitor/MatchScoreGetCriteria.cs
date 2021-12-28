namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MatchScoreGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_MatchID")]
        public Int64 MatchID { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID")]
        public int BettypeID { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator")]
        public bool LiveIndicator { get; set; }

        [SpParam(SqlParamName = "ip_IsHedging")]
        public bool IsHedging { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_Details_GetScore";

        public override bool IsValid()
            => true;
    }
}
