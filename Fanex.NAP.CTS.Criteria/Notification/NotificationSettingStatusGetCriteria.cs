namespace Fanex.NAP.CTS.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NotificationSettingStatusGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public long UserID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Notification_GetSettings";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
