namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerCategoryByCTSCustIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }
     
        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_GetCategory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
