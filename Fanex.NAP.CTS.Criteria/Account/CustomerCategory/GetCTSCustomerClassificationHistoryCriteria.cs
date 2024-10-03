namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class GetCTSCustomerClassificationHistoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustID { get; set; }

        [SpParam(Ignored = true)]
        public ROLE_CUSTOMER RoleID { get; set; }

        public override string GetSettingKey()
        {
            return this.RoleID == ROLE_CUSTOMER.MEMBER
                ? "CTS_DC_CustClassification_ViewHistory"
                : "CTS_DC_CustClassificationAgency_ViewHistory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
