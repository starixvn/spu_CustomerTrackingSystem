namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetEvidenceByCustIDCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public int CustId { get; set; }

        [SpParam(SqlParamName = "ip_EvidenceCodes")]
        public string EvidenceCodes { get; set; }

        [SpParam(SqlParamName = "ip_IsFlagged")]
        public bool IsFlagged { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_GetEvidenceByCustID";

        public override bool IsValid()
            => true;

    }
}