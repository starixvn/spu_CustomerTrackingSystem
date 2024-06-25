namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetProblemAccountCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsFromPAM")]
        public bool IsFromPAM { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_ProblemAccountManagement_GetCategories";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
