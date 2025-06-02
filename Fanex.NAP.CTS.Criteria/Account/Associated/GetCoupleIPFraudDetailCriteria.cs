namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    public class GetCoupleIPFraudDetailCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CoupleInfoDetails")]
        public string CoupleInfoDetails { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_GB_3S_GetCoupleIP_FraudDetails";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
