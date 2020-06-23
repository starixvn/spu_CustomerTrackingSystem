namespace Fanex.NAP.CTS.Criteria.ProblemAccountMonitor
{
    using Fanex.Data.Repository;

    public class DetailTicketsGetCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public int FraudCaseId { get; set; }

        public string SiteIds { get; set; }

        public int BettypeId { get; set; }

        public int CurrencyId { get; set; }

        public decimal StakeFrom { get; set; }

        public decimal StakeTo { get; set; }

        public override string GetSettingKey()
            => "FS_Rpt_CTSMatchMonitor_Details_GetTickets";

        public override bool IsValid()
            => true;
    }
}