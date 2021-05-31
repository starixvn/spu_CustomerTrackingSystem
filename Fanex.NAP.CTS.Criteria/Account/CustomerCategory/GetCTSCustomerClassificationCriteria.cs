namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCTSCustomerClassificationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
