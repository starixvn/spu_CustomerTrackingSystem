namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class MMParlayFraudTicketsSummaryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "FraudTickets")]
        public string FraudTickets { get; set; }

        public override string GetSettingKey() => "CTS_MatchMonitorParlay_FraudTicketSummary";

        public override bool IsValid() => true;
    }
}
