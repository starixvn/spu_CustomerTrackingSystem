namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDirectAssociatedAccountTagFlaggedCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustJson")]
        public string CustAssInfo { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidence_GetFlaggedStatus";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
