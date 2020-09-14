namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetRemoveEvidenceAssociatedAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public string CTSCustIdList { get; set; }

        [SpParam(SqlParamName = "ip_EvidenceID")]
        public int EvidenceID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_GetCleanAssociatedCustExceptAEvidence";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
