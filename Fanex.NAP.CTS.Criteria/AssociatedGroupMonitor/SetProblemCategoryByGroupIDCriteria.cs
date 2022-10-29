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

        [SpParam(SqlParamName = "ip_PAAllCredit")]
        public int PAAllCredit { get; set; }

        [SpParam(SqlParamName = "ip_PAAllLicensee")]
        public int PAAllLicensee { get; set; }

        [SpParam(SqlParamName = "ip_PASites")]
        public string PASites { get; set; }

        [SpParam(SqlParamName = "ip_ModifiedBy")]
        public int CreatedBy { get; set; }

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
