namespace Fanex.NAP.CTS.Criteria.Account.RobotSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;    

    public class GetRobotDetectionTicketDetailsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ListTransId")]
        public string ListTransId { get; set; }

        [SpParam(SqlParamName = "MatchBatchSize")]
        public int MatchBatchSize { get; set; }
     
        public override string GetSettingKey()
        {
            return "CTS_Rpt_RobotDetection_TicketDetails";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
