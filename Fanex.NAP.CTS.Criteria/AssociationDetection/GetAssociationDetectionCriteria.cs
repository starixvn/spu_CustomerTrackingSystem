using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    public class GetAssociationDetectionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs")]
        public string CTSCustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_RPT_AssociationDetection";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
