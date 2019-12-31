using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    public class GetDirectAssociationByAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_custIDs")]
        public string CustIds { get; set; }
                
        public override string GetSettingKey()
        {
            return "CTS_DC_GetDirectAssociatedAccount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
