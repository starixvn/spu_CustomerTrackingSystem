using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.AssociatedAccountMonitor
{
    public class AssociatedAccountMonitorGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "CTS_DC_AssociatedAccountMonitor_Get";

        public override bool IsValid()
            => true;
    }
}
