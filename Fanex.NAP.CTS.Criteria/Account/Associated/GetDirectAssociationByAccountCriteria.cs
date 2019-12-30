using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    public class GetDirectAssociationByAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ctsCustID")]
        public int CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_associationTypeID")]
        public int AssociationType { get; set; }

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
