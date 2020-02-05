namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAffectedEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public int CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_EvidenceCodes")]
        public string EvidenceCodes { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_GetAffectedEvidence";

        public override bool IsValid()
            => true;

    }
}