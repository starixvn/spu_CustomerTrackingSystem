using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    public class GetCTSCustomerClassificationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetCTSCustomerClassification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
