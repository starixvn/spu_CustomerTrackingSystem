namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetFlaggedEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public Int64 CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_EvidenceCodes", AllowNull = true)]
        public string EvidenceCodes { get; set; }       

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_GetFlaggedEvidences";

        public override bool IsValid()
            => true;

    }
}
