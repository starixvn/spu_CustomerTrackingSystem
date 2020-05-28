namespace Fanex.NAP.CTS.Criteria.ProblemAccountMonitor
{
    using Fanex.Data.Repository;    

    public class DetailMatchInfoGetCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public int FraudCaseId { get; set; }

        public override string GetSettingKey()
            => "FS_Rpt_CTSMatchMonitor_Details_GetMatchInfo";

        public override bool IsValid()
            => true;
    }
}
