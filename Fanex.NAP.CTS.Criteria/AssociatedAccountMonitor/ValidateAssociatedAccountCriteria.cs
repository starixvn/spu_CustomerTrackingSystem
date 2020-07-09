namespace Fanex.NAP.CTS.Criteria.AssociatedAccount
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ValidateAssociatedAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UsernameList")]
        public string UserNameList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_InsertAssociatedAccountMonitorValidation";

        public override bool IsValid()
            => true;

    }
}
