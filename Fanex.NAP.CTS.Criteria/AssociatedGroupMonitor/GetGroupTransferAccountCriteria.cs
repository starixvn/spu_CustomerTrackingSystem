namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetGroupTransferAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_GroupID")]
        public long GroupID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_TransferAccount_GetGroup";
        }

        public override bool IsValid()
        {
            return this.GroupID != 0;
        }
    }
}
