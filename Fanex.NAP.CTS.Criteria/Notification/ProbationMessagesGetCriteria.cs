namespace Fanex.NAP.CTS.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProbationMessageGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public long UserID { get; set; }

        [SpParam(SqlParamName = "ip_ReadNotificationID")]
        public long MaxNotificationID { get; set; }

        public ProbationMessageGetCriteria(long userId, long maxNotificationID)
        {
            UserID = userId;
            MaxNotificationID = maxNotificationID;
        }

        public override string GetSettingKey()
        {
            return "CTS_DC_Notification_GetProbation";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}