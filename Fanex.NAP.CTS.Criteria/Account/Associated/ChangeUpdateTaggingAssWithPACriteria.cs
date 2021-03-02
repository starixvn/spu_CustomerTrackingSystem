namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ChangeUpdateTaggingAssWithPACriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public int CustId { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "ip_IsExcludedTagAssPA")]
        public bool IsExcludedTagAssPA { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
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
