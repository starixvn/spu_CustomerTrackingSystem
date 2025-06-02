namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateMMUserFeedbackCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "ip_MMUserCommentID")]
        public long MMUserCommentID { get; set; }

        [SpParam(SqlParamName = "ip_UserComment")]
        public string UserComment { get; set; }

        [SpParam(SqlParamName = "ip_CorrectReason")]
        public int CorrectReasonID { get; set; }

        [SpParam(SqlParamName = "ip_IncorrectReason")]
        public string IncorrectReason { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_UserComment_Update";

        public override bool IsValid()
            => true;

    }
}
