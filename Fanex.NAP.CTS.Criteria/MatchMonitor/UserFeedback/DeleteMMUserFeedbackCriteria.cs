namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DeleteMMUserFeedbackCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "ip_MMUserCommentID")]
        public long MMUserCommentID { get; set; }
     
        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_UserComment_Delete";

        public override bool IsValid()
            => true;
    }
}
