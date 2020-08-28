namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertAccountEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NewAccountEvidences")]
        public string NewAccountEvidences { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "ip_IsOverride")]
        public bool IsOverride { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_AddMultiCustomerEvidence";

        public override bool IsValid()
            => true;

    }
}