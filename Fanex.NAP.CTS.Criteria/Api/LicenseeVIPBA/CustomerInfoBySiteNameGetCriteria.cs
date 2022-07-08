namespace Fanex.NAP.CTS.Criteria.Api
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustomerInfoBySiteNameGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SiteName")]
        public string SiteName { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_GetBySiteName";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
