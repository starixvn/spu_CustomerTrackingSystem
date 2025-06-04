namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AddAssociatedAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_GroupID", AllowNull = true)]
        public long GroupID { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustIDs", AllowNull = true)]
        public string CTSCustIDs { get; set; }

        [SpParam(SqlParamName = "ip_IsAuto", AllowNull = true)]
        public bool IsAuto { get; set; }

        [SpParam(SqlParamName = "ip_Remark", AllowNull = true)]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy", AllowNull = true)]
        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_Account_Add";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
