namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MatchDetailMonitorGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_MatchID", AllowNull = true)]
        public Int64 MatchID { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator", AllowNull = true)]
        public bool LiveIndicator { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID", AllowNull = true)]
        public int BettypeID { get; set; }

        [SpParam(SqlParamName = "ip_BetIDList", AllowNull = true)]
        public string BetID { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_Details_Get";

        public override bool IsValid()
            => true;
    }
}
