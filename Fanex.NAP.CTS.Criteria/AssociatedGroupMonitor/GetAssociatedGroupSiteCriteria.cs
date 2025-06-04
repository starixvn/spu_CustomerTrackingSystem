namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociatedGroupSiteCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_GroupID", AllowNull = true)]
        public long GroupID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_GetSites";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
