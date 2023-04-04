namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetTicketDetailsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ListTransId")]
        public string ListTransId { get; set; }

        [SpParam(SqlParamName = "MatchSize")]
        public int? MatchSize { get; set; }

        [SpParam(SqlParamName = "TransSize")]
        public int? TransSize { get; set; }

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
