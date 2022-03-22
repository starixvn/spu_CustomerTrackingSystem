namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ValidateUserNamesInputCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserNames")]
        public string UserNames { get; set; }

        [SpParam(SqlParamName = "ip_CategoryID")]
        public int CategoryID { get; set; }

        [SpParam(SqlParamName = "ip_IsInsertingPA")]
        public bool IsInsertingPA { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_ProblemAccountManagement_ValidateMultipleInput";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
