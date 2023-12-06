namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class MMFraudTicketsSummaryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "FraudTickets")]
        public string FraudTickets { get; set; }

        public override string GetSettingKey() => "CTS_Rpt_MatchMonitor_FraudTicketsSummary";

        public override bool IsValid() => true;
    }
}
