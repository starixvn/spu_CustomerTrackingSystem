namespace Fanex.NAP.CTS.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NotificationSettingStatusGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public long UserID { get; set; }

        public override string GetSettingKey()
        {
            return "";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}