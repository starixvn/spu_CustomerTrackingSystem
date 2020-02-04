namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetFlaggedEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public int CustId { get; set; }

        [SpParam(SqlParamName = "ip_EvidenceCodes")]
        public string EvidenceCodes { get; set; }       

        public override string GetSettingKey()
            => "CTS_DC_GetFlaggedEvidence";

        public override bool IsValid()
            => true;

    }
}