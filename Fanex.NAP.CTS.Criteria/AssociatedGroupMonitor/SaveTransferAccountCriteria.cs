namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SaveTransferAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs")]
        public string CTSCustIDs { get; set; }

        [SpParam(SqlParamName = "ip_FromGroupID")]
        public long FromGroupID { get; set; }

        [SpParam(SqlParamName = "ip_ToGroupID")]
        public long ToGroupID { get; set; }

        [SpParam(SqlParamName = "ip_ModifiedBy")]
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
