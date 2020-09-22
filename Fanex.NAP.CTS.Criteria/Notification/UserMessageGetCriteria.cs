namespace Fanex.NAP.CTS.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UserMessageGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "ip_MaxNewAssID")]
        public int MaxNewAssID { get; set; }

        public UserMessageGetCriteria(int userId, int aMaxNewAssID)
        {
            UserID = userId;
            MaxNewAssID = aMaxNewAssID;
        }

        public override string GetSettingKey()
        {
            return "CTS_DC_Notification_GetAssociatedAccountNewAssociation";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}