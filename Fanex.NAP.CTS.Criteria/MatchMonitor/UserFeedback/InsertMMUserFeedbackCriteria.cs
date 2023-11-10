namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertMMUserFeedbackCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "ip_MatchID")]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "ip_SportType")]
        public int SportType { get; set; }

        [SpParam(SqlParamName = "ip_LiveIndicator")]
        public bool LiveIndicator { get; set; }

        [SpParam(SqlParamName = "ip_BettypeID")]
        public int BettypeID { get; set; }

        [SpParam(SqlParamName = "ip_BetID")]
        public int BetID { get; set; }

        [SpParam(SqlParamName = "ip_UserComment")]
        public string UserComment { get; set; }

        [SpParam(SqlParamName = "ip_CorrectReason")]
        public int CorrectReasonID { get; set; }

        [SpParam(SqlParamName = "ip_IncorrectReason")]
        public string IncorrectReason { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_UserComment_Insert";

        public override bool IsValid()
            => true;
    }
}
