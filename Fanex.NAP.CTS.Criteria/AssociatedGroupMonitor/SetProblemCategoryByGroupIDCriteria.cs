namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SetProblemCategoryByGroupIDCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_GroupID", AllowNull = true)]
        public long GroupID { get; set; }

        [SpParam(SqlParamName = "ip_PACategoryID", AllowNull = true)]
        public int PACategoryID { get; set; }

        [SpParam(SqlParamName = "ip_PACreditSites", AllowNull = true)]
        public string PACreditSites { get; set; }

        [SpParam(SqlParamName = "ip_PALicenseeSites", AllowNull = true)]
        public string PALicenseeSites { get; set; }       

        [SpParam(SqlParamName = "ip_ModifiedBy", AllowNull = true)]
        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_SetProblemCategoryByGroupID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
