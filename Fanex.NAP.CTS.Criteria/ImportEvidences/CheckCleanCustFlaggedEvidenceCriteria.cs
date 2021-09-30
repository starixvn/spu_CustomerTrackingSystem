namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckCleanCustFlaggedEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustIDList")]
        public string CTSCustIds { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_CheckCleanCust";

        public override bool IsValid()
            => true;
    }
}