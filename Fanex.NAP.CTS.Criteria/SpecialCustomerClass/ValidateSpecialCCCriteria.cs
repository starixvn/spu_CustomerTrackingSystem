namespace Fanex.NAP.CTS.Criteria.SpecialCustomerClass
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ValidateSpecialCCCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ActionType", AllowNull = true)]
        public int ActionType { get; set; }

        [SpParam(SqlParamName = "ip_ListUser", AllowNull = true)]
        public string ListUser { get; set; }

        public override string GetSettingKey() => "CTS_DC_SpecialCustClass_ValidateUsername";

        public override bool IsValid() => true;
    }
}
