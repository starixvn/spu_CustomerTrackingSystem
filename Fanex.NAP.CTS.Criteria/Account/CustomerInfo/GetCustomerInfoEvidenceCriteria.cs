namespace Fanex.NAP.CTS.Criteria.Account.CustomerInfo
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerInfoEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public long CTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_Profile_GetEvidence";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
