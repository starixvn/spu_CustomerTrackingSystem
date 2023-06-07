namespace Fanex.NAP.CTS.Criteria.Api.FPS.Transaction
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetLastestTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_Length")]
        public int Length { get; set; }

        [SpParam(SqlParamName = "ip_Skip")]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "ip_Take")]
        public int Take { get; set; }

        [SpParam(SqlParamName = "op_TotalItem")]
        public int TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transaction_GetLatestBySubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
