namespace Fanex.NAP.CTS.Criteria.Account.CustomerInfo
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerInfoAssociatedAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_Profile_GetAssociationAccount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
