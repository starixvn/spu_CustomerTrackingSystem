namespace Fanex.NAP.CTS.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AssociatedMessagesGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public long UserID { get; set; }

        [SpParam(SqlParamName = "ip_MaxNewAssID", AllowNull = true)]
        public long MaxNewAssID { get; set; }

        public AssociatedMessagesGetCriteria(long userId, long aMaxNewAssID)
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
