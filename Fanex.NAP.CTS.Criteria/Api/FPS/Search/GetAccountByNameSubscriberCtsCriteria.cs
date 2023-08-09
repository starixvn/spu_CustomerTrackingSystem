namespace Fanex.NAP.CTS.Criteria.Api.FPS.SearchAccount
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAccountByNameSubscriberCtsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_UserName")]
        public string UserName { get; set; }      

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetByNameSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
