namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data.Repository;

    public class GetAssociatedGroupMonitorCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
