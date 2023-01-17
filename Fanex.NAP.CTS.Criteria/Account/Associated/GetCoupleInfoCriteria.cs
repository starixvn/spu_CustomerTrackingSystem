
namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data.Repository;

    public class GetCoupleInfoCriteria : CriteriaBase
    {
        public string ip_CustID1 { get; set; }

        public string ip_CustID2 { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_GB_3S_GetCoupleInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
