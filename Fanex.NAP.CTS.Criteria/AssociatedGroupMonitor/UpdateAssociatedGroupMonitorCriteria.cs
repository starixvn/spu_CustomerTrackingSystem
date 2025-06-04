namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateAssociatedGroupMonitorCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_GroupID", AllowNull = true)]
        public long GroupID { get; set; }

        [SpParam(SqlParamName = "ip_GroupName", AllowNull = true)]
        public string GroupName { get; set; }

        [SpParam(SqlParamName = "ip_ABI", AllowNull = true)]
        public int ABI { get; set; }

        [SpParam(SqlParamName = "ip_Danger1", AllowNull = true)]
        public int Ori { get; set; }

        [SpParam(SqlParamName = "ip_AllCredit", AllowNull = true)]
        public int AllCredit { get; set; }

        [SpParam(SqlParamName = "ip_AllLicensee", AllowNull = true)]
        public int AllLicensee { get; set; }

        [SpParam(SqlParamName = "ip_Sites", AllowNull = true)]
        public string Sites { get; set; }

        [SpParam(SqlParamName = "ip_HasDevice", AllowNull = true)]
        public bool HasDevice { get; set; }

        [SpParam(SqlParamName = "ip_HasManual", AllowNull = true)]
        public bool HasManual { get; set; }

        [SpParam(SqlParamName = "ip_HasIP", AllowNull = true)]
        public bool HasIP { get; set; }

        [SpParam(SqlParamName = "ip_HasAI", AllowNull = true)]
        public bool HasAI { get; set; }

        [SpParam(SqlParamName = "ip_Remark", AllowNull = true)]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_ModifiedBy", AllowNull = true)]
        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
