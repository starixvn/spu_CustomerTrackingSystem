namespace Fanex.NAP.CTS.Criteria.Transaction
{
    using Fanex.Data.Repository;
    public class GetTicketDetailsCriteria : CriteriaBase
    {
        public string ListTransId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_TicketDetails_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
