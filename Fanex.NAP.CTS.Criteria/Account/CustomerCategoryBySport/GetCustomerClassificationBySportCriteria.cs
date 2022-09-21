namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategoryBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
   
    public class GetCustomerClassificationBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
