namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ValidateUserNamesInputCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserNames")]
        public string UserNames { get; set; }
      
        public override string GetSettingKey()
        {
            return "CTS_DC_ProblemAccountManagement_ValidateUserName";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
