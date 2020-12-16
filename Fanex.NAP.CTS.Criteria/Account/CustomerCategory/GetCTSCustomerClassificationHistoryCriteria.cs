namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCTSCustomerClassificationHistoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public long CustId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_ViewHistory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
