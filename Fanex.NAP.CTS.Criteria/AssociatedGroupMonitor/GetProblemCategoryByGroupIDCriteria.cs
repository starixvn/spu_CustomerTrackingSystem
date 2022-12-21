namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetProblemCategoryByGroupIDCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_GroupID")]
        public long GroupID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_GetProblemCategoryByGroupID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
