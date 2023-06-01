namespace Fanex.NAP.CTS.Criteria.Api.SearchAccount
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAccountByNameInSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_UserNameList")]
        public string UserNameList { get; set; }      

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
