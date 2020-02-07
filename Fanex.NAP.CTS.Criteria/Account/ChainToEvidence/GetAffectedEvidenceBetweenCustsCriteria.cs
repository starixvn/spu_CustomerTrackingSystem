namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAffectedEvidenceBetweenCustsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromCTSCustID")]
        public int FromCTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_ToCTSCustID")]
        public int ToCTSCustID { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_GetAffectedEvidenListBetweenCusts";       

        public override bool IsValid()
            => true;       
    }
}