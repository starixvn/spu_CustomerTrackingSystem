using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account.ChainToEvidence
{
    public class GetAccountExceptionsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public int CTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetExceptionByCTSCustID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
