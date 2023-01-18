
namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetCoupleFraudDetailCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CoupleID")]
        public long CoupleID { get; set; }

        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_GB_3S_GetCouple_FraudDetails";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
