namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveOldEvidencesCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs")]
        public string NewAccountEvidences { get; set; }

        [SpParam(SqlParamName = "ip_Timetemp")]
        public long TimeSpan { get; set; }

        [SpParam(SqlParamName = "ip_Rollback")]
        public bool IsRollBack { get; set; }

        [SpParam(SqlParamName = "ip_ClearHistory")]
        public bool IsClearLog { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_RemoveOldEvidence";

        public override bool IsValid()
            => true;

    }
}