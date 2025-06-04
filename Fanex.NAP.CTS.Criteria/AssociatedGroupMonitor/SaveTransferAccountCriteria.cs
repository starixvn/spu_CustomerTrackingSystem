namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SaveTransferAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs", AllowNull = true)]
        public string CTSCustIDs { get; set; }

        [SpParam(SqlParamName = "ip_FromGroupID", AllowNull = true)]
        public long FromGroupID { get; set; }

        [SpParam(SqlParamName = "ip_ToGroupID", AllowNull = true)]
        public long ToGroupID { get; set; }

        [SpParam(SqlParamName = "ip_ModifiedBy", AllowNull = true)]
        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_TransferAccount_Update";
        }

        public override bool IsValid()
        {
            return this.FromGroupID != 0 && this.ToGroupID != 0;
        }
    }
}
