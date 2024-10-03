namespace Fanex.NAP.CTS.Criteria.Account.CustomerInfo
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class GetCustomerInfoDateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustID { get; set; }

        [SpParam(Ignored = true)]
        public ROLE_CUSTOMER RoleID { get; set; }

        public override string GetSettingKey()
        {
            return this.RoleID == ROLE_CUSTOMER.MEMBER 
                ? "CTS_DC_CustInfo_Profile_GetDate"
                : "CTS_DC_CustInfoAgency_Profile_GetDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
