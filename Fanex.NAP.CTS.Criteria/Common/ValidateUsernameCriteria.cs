namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ValidateUsernameCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListUsername", AllowNull = true)]
        public string userNames { get; set; }

        public override string GetSettingKey() => "CTS_DC_Common_ValidateUsername";

        public override bool IsValid() => true;
    }
}
