namespace Fanex.NAP.CTS.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AssociatedMessagesGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "ip_MaxNewAssID")]
        public long MaxNewAssID { get; set; }

        public AssociatedMessagesGetCriteria(int userId, long aMaxNewAssID)
        {
            UserID = userId;
            MaxNewAssID = aMaxNewAssID;
        }

        public override string GetSettingKey()
        {
            return "CTS_DC_Notification_GetNewAssociation";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}