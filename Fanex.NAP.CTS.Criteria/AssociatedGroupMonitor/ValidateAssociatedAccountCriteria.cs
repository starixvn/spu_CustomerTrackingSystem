namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ValidateAssociatedAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ListUsername", AllowNull = true)]
        public string UserNames { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_ValidateUsername";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
