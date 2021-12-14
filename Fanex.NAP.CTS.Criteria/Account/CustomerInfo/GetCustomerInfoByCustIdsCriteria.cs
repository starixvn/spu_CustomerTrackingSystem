namespace Fanex.NAP.CTS.Criteria.Account.CustomerInfo
{
    using Fanex.Data;
    using Fanex.Data.Repository;    

    public class GetCustomerInfoByCustIdsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "custIdList")]
        public string CustIdList { get; set; }

        [SpParam(SqlParamName = "sourceType")]
        public int SourceType { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_GetInfoByCustIdList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
