namespace Fanex.NAP.CTS.Criteria.Api.FPS.SearchAccount
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetListAccountByUserNameSubscriberCtsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_UserName")]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "ip_Skip")]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "ip_Take")]
        public int Take { get; set; }

        [SpParam(SqlParamName = "op_TotalItem")]
        public int TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetPartialByUserNameSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
