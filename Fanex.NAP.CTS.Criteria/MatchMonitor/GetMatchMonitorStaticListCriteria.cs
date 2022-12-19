namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data.Repository;

    public class GetMatchMonitorStaticListCriteria : CriteriaBase
    {      
        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_GetStaticList";

        public override bool IsValid()
            => true;
    }
}
