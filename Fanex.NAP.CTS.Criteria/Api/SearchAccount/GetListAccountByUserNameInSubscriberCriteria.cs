namespace Fanex.NAP.CTS.Criteria.Api.SearchAccount
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetListAccountByUserNameInSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_UserNameList")]
        public string UserNameList { get; set; }

        [SpParam(SqlParamName = "ip_IsPartial")]
        public bool IsPartial { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Common_GetStaticList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
