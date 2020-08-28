namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckAccountEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NewAccountEvidences")]
        public string NewAccountEvidences { get; set; }

        [SpParam(SqlParamName = "ip_IsOverride")]
        public bool IsOverride { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CheckMultiCustomerEvidence";

        public override bool IsValid()
            => true;
    }
}