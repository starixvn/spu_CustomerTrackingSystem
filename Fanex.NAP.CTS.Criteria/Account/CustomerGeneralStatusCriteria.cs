using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account
{
    public class CustomerGeneralStatusCriteria : CriteriaBase
    {
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerGeneralStatus_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
