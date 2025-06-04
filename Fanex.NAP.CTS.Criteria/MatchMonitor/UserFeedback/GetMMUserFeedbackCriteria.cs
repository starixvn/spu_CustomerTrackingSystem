namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMMUserFeedbackCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_MatchID", AllowNull = true)]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator", AllowNull = true)]
        public bool LiveIndicator { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID", AllowNull = true)]
        public int BetTypeID { get; set; }

        [SpParam(SqlParamName = "ip_BetIDList", AllowNull = true)]
        public string BetIDs { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_UserComment_Get";

        public override bool IsValid()
            => true;

    }
}
