namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class RemoveFlaggedEvidenceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public Int64 CTSCustId { get; set; }       

        [SpParam(SqlParamName = "ip_EvidenceID", AllowNull = true)]
        public int EvidenceId { get; set; }

        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_RemoveFlagged";

        public override bool IsValid() => true;
    }
}
