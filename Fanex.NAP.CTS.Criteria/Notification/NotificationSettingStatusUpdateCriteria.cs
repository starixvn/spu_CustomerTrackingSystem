namespace Fanex.NAP.CTS.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NotificationSettingStatusUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public long UserID { get; set; }

        [SpParam(SqlParamName = "ip_FunctionID", AllowNull = true)]
        public int FunctionID { get; set; }

        [SpParam(SqlParamName = "ip_IsTurningOn", AllowNull = true)]
        public bool IsTurningOnPermission { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Notification_UpdateSettings";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
