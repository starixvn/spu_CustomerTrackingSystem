namespace Fanex.NAP.CTS.Criteria.ProblemMatchMonitor
{
    using Fanex.Data.Repository;

    public class DetailTicketsGetCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public int FraudCaseId { get; set; }

        public override string GetSettingKey()
            => "FS_Rpt_CTSMatchMonitor_Details_GetTickets";

        public override bool IsValid()
            => true;
    }
}
