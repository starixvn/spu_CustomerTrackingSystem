namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SetProblemCategoryByGroupIDCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_GroupID")]
        public long GroupID { get; set; }

        [SpParam(SqlParamName = "ip_PACategoryID")]
        public int PACategoryID { get; set; }

        [SpParam(SqlParamName = "ip_PACreditSites")]
        public string PACreditSites { get; set; }

        [SpParam(SqlParamName = "ip_PALicenseeSites")]
        public string PALicenseeSites { get; set; }       

        [SpParam(SqlParamName = "ip_ModifiedBy")]
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
