namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetAffectedEvidenceBetweenCustsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromCTSCustID")]
        public Int64 FromCTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_ToCTSCustID")]
        public Int64 ToCTSCustID { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_GetAffectedEvidenListBetweenCusts";       

        public override bool IsValid()
            => true;       
    }
}