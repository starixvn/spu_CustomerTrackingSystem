namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociatedAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_GroupID", AllowNull = true)]
        public long GroupID { get; set; }

        [SpParam(SqlParamName = "op_AllCredit", AllowNull = true)]
        public int AllCredit { get; set; }

        [SpParam(SqlParamName = "op_AllLicensee", AllowNull = true)]
        public int AllLicensee { get; set; }

        [SpParam(SqlParamName = "op_Sites", AllowNull = true)]
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
