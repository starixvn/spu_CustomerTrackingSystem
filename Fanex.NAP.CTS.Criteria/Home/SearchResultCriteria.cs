namespace Fanex.NAP.CTS.Criteria.Home
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SearchResultCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_username")]
        public string Username { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetByUsername";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
