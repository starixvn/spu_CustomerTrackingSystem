namespace Fanex.NAP.CTS.Criteria.Account.CustomerInfo
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerInfoDateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_Profile_GetDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
