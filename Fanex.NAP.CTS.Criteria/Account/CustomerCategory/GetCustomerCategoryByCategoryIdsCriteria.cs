namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerCategoryByCategoryIdsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CategoryIDList")]
        public string CategoryIDList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_GetAllByCategory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
