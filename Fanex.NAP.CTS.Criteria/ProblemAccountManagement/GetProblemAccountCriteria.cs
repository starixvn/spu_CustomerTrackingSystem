namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetProblemAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate", AllowNull = true)]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate", AllowNull = true)]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_Sites", AllowNull = true)]
        public string Sites { get; set; }

        [SpParam(SqlParamName = "ip_CategoryIDs", AllowNull = true)]
        public string CategoryIDs { get; set; }

        [SpParam(SqlParamName = "ip_UserName", AllowNull = true)]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "ip_RoleIDs", AllowNull = true)]
        public string RoleIDs { get; set; }

        [SpParam(SqlParamName = "ip_HasDownline", AllowNull = true)]
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
