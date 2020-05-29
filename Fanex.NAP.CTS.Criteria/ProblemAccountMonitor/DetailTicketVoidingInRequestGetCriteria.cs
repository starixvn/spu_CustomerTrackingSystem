namespace Fanex.NAP.CTS.Criteria.ProblemAccountMonitor
{
    using Fanex.Data.Repository;

    public class DetailTicketVoidingInRequestGetCriteria : CriteriaBase
    {
        public string TransIds { get; set; }

        public int MatchId { get; set; }

        public override string GetSettingKey()
            => "TicketVoiding_TicketsInRequest_Select";

        public override bool IsValid()
            => true;

    }
}