namespace Fanex.NAP.CTS.Criteria.TWProblemAccountDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustClassificationBySiteCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SiteIDList")]
        public string SiteIdList { get; set; }
        
        [SpParam(SqlParamName = "ip_UserNameList")]
        public string UsernameList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_GetCustClassificationBySite";

        public override bool IsValid()
            => true;
    }
}