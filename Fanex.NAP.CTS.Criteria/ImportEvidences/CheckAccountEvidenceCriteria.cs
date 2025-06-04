namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckAccountEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NewAccountEvidences", AllowNull = true)]
        public string NewAccountEvidences { get; set; }

        [SpParam(SqlParamName = "ip_IsOverride", AllowNull = true)]
        public bool IsOverride { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_ValidateMultipleInput";

        public override bool IsValid()
            => true;
    }
}
