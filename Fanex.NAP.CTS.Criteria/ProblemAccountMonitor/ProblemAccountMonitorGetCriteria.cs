namespace Fanex.NAP.CTS.Criteria.ProblemAccountMonitor
{
    using Fanex.Data.Repository;
    using System;

    public class ProblemAccountMonitorGetCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public string LeagueIds { get; set; }

        public string MatchStatus { get; set; } // All, completed, running

        public string SiteIds { get; set; }

        public override string GetSettingKey()
           => "FS_Rpt_CTSMatchMonitor_Get";

        public override bool IsValid()
            => true;
    }
}