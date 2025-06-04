namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateMMUserFeedbackCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "ip_MMUserCommentID", AllowNull = true)]
        public long MMUserCommentID { get; set; }

        [SpParam(SqlParamName = "ip_UserComment", AllowNull = true)]
        public string UserComment { get; set; }

        [SpParam(SqlParamName = "ip_CorrectReason", AllowNull = true)]
        public int CorrectReasonID { get; set; }

        [SpParam(SqlParamName = "ip_IncorrectReason", AllowNull = true)]
        public string IncorrectReason { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_UserComment_Update";

        public override bool IsValid()
            => true;

    }
}
