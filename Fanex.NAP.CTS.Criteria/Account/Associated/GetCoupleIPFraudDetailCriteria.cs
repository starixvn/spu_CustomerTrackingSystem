namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data.Repository;
    using System;

    public class GetCoupleIPFraudDetailCriteria : CriteriaBase
    {
        public string ip_CoupleID { get; set; }
        public DateTime ip_FromDate { get; set; }
        public DateTime ip_ToDate { get; set; }
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
