namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetMMUserFeedbackCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_MatchID")]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator")]
        public bool LiveIndicator { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID")]
        public int BetTypeID { get; set; }

        [SpParam(SqlParamName = "ip_BetIDList")]
        public string BetIDs { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_UserComment_Get";

        public override bool IsValid()
            => true;

    }
}
