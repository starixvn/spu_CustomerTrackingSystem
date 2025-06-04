namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ChangeUpdateTaggingAssWithPACriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy", AllowNull = true)]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "ip_IsExcludedTagAssPA", AllowNull = true)]
        public bool IsExcludedTagAssPA { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedAccountMonitor_UpdateTaggingAssWithPA";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
