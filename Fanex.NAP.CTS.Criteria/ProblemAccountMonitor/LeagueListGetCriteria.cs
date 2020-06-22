namespace Fanex.NAP.CTS.Criteria.ProblemAccountMonitor
{
    using Fanex.Data.Repository;
    using System;

    public class LeagueListGetCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public string SportIds { get; set; }

        public override string GetSettingKey()
            => "FS_CTSLeagueList_GetBySports";

        public override bool IsValid()
            => true;
    }
}