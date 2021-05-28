namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class PinCTSCategoryCustomerClassificationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "ip_IsPinCategory")]
        public bool IsPinCategory { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_AssociatedAccountMonitor_UpdatePin";

        public override bool IsValid()
            => true;
    }
}
