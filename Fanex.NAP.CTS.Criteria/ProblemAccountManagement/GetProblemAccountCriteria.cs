namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetProblemAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_Sites")]
        public string Sites { get; set; }

        [SpParam(SqlParamName = "ip_CategoryIDs")]
        public string CategoryIDs { get; set; }

        [SpParam(SqlParamName = "ip_UserName")]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "ip_RoleIDs")]
        public string RoleIDs { get; set; }

        [SpParam(SqlParamName = "ip_HasDownline")]
        public bool HasDownline { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_ProblemAccountManagement_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
