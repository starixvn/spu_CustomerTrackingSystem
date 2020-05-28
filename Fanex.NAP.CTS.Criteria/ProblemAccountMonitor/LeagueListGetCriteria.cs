namespace Fanex.NAP.CTS.Criteria.ProblemAccountMonitor
{
    using Fanex.Data.Repository;
    using System;

    public class LeagueListGetCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public int SportId { get; set; }

        public override string GetSettingKey()
            => "FS_League_Get";

        public override bool IsValid()
            => true;
    }
}