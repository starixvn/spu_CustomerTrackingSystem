namespace Fanex.NAP.CTS.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NotificationSettingStatusUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public long UserID { get; set; }

        [SpParam(SqlParamName = "ip_FunctionID")]
        public int FunctionID { get; set; }

        [SpParam(SqlParamName = "ip_IsTurningOn")]
        public bool IsTurningOnPermission { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
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