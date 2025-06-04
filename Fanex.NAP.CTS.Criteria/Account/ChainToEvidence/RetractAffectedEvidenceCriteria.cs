namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System;

    public class RetractAffectedEvidenceCriteria : CommandBase
    {
        [SpParam(Ignored = true)]
        public override int ErrCode { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public Int64 CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_EvidenceID", AllowNull = true)]
        public int EvidenceId { get; set; }

        [SpParam(SqlParamName = "ip_Remark", AllowNull = true)]
        public string Comment { get; set; }

        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public int UserId { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_RemoveAffected";

        public override bool IsValid()
            => true;
    }
}
