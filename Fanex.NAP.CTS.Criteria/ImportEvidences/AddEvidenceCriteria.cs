namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AddEvidenceCriteria : CriteriaBase
    {
        public string UserName { get; set; }

        public string EvidenceCode { get; set; }

        public string Comment { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "";

        public override bool IsValid()
            => true;
    }
}