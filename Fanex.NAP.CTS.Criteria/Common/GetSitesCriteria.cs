namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetSitesCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsNestedCalled", AllowNull = true)]
        public bool IsNestedCalled { get; set; }

        public override string GetSettingKey()
           => "CTS_DC_Common_GetSites";

        public override bool IsValid()
            => true;
    }
}
