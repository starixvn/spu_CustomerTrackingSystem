namespace Fanex.NAP.CTS.Criteria.Api.FPS.SearchAccount
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAccountByNameSubscriberDcsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_LoginName")]
        public string UserName { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_AccountInfo_GetByLoginNameSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
