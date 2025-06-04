namespace Fanex.NAP.CTS.Criteria.ImportEvidences
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveOldEvidencesCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs", AllowNull = true)]
        public string NewAccountEvidences { get; set; }

        [SpParam(SqlParamName = "ip_SessionId", AllowNull = true)]
        public long SessionId { get; set; }

        [SpParam(SqlParamName = "ip_IsRollback", AllowNull = true)]
        public bool IsRollBack { get; set; }

        [SpParam(SqlParamName = "ip_IsClearLog", AllowNull = true)]
        public bool IsClearLog { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_RemoveAllEvidenceByCTSCustIDs";

        public override bool IsValid()
            => true;

    }
}
