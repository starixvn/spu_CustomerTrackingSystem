namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AddAssociatedGroupMonitorCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_GroupName")]
        public string GroupName { get; set; }

        [SpParam(SqlParamName = "ip_ABI")]
        public int ABI { get; set; }

        [SpParam(SqlParamName = "ip_Remark")]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_Create";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
