namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveAssociatedGroupMonitorCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_GroupID", AllowNull = true)]
        public long GroupID { get; set; }

        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public int UserID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_Remove";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
