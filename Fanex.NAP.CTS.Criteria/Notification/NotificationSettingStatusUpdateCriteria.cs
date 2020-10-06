namespace Fanex.NAP.CTS.Criteria.Notification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NotificationSettingStatusUpdateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public long UserID { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public int FuncId { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public bool IsTurnOnPermission { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

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