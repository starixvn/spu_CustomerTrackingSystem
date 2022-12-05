namespace Fanex.NAP.CTS.Criteria.Account.CustomerInfo
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerInfoDangerousCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetCustInfoDangerous";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
