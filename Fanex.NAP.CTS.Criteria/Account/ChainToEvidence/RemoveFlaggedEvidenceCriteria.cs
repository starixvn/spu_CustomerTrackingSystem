namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class RemoveFlaggedEvidenceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public Int64 CTSCustId { get; set; }       

        [SpParam(SqlParamName = "ip_EvidenceID")]
        public int EvidenceId { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_RemoveFlagged";

        public override bool IsValid() => true;
    }
}