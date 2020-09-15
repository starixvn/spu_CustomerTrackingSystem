using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    public class GetCTSCustomerClassificationForAPICriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIdList { get; set; }

        [SpParam(SqlParamName = "ip_ParentCateID")]
        public int ParentCateId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Get4API";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
