namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDirectAssociatedAccountTagAffectedCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_AssCTSCustJson")]
        public string CustAssInfo { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetDirectAssociatedAccount_TagAffected";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
