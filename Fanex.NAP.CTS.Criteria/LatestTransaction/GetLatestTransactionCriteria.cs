namespace Fanex.NAP.CTS.Criteria.LatestTransaction
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetLatestTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberId")]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_Length")]
        public int Length { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transaction_GetLatest";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
