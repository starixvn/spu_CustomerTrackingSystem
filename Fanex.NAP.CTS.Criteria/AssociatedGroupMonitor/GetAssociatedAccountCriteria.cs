namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociatedAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_GroupID")]
        public long GroupID { get; set; }

        [SpParam(SqlParamName = "op_AllCredit")]
        public int AllCredit { get; set; }

        [SpParam(SqlParamName = "op_AllLicensee")]
        public int AllLicensee { get; set; }

        [SpParam(SqlParamName = "op_Sites")]
        public string Sites { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_Account_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
