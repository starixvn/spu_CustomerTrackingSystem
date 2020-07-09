using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.AssociatedAccountMonitor
{
    public class AssociatedAccountMonitorGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CTS_DC_RPT_AssociatedAccountMonitor";

        public override bool IsValid()
            => true;
    }
}
