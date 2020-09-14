namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System;

    public class PlaceEvidenceCriteria : CommandBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public Int64 CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_EvidenceID")]
        public int EvidenceId { get; set; }

        [SpParam(SqlParamName = "ip_Remark")]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public int UserId { get; set; }

        [SpParam(Ignored = true)]
        public override int ErrCode { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_InsertSingle";

        public override bool IsValid() => true;
    }
}