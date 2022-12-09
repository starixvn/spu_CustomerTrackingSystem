namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ValidateUsernameTransferAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListUsername")]
        public string UserNames { get; set; }

        [SpParam(SqlParamName = "ip_GroupID")]
        public long GroupID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_TransferAccount_ValidateUsername";
        }

        public override bool IsValid()
        {
            return this.GroupID != 0;
        }
    }
}
