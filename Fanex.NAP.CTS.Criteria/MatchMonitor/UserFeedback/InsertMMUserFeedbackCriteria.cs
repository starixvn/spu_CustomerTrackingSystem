namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertMMUserFeedbackCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "ip_MatchID", AllowNull = true)]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "ip_SportType", AllowNull = true)]
        public int SportType { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator", AllowNull = true)]
        public bool LiveIndicator { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID", AllowNull = true)]
        public int BettypeID { get; set; }

        [SpParam(SqlParamName = "ip_BetIDList", AllowNull = true)]
        public string BetIDs { get; set; }

        [SpParam(SqlParamName = "ip_UserComment", AllowNull = true)]
        public string UserComment { get; set; }

        [SpParam(SqlParamName = "ip_CorrectReason", AllowNull = true)]
        public int CorrectReasonID { get; set; }

        [SpParam(SqlParamName = "ip_IncorrectReason", AllowNull = true)]
        public string IncorrectReason { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_UserComment_Insert";

        public override bool IsValid()
            => true;
    }
}
