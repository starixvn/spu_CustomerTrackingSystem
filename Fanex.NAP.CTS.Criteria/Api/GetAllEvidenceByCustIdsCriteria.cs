using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Api
{
    public class GetAllEvidenceByCustIdsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidence_GetEvidencesByDirectAssociation";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
