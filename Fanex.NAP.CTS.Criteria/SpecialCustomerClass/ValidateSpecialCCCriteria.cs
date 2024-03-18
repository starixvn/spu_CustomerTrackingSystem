namespace Fanex.NAP.CTS.Criteria.SpecialCustomerClass
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ValidateSpecialCCCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FunctionID")]
        public int FunctionValidateSpecialCC { get; set; }

        [SpParam(SqlParamName = "ip_ListUser")]
        public string ListUser { get; set; }

        public override string GetSettingKey() => "CTS_DC_SpecialCustClass_ValidateUsername";

        public override bool IsValid() => true;
    }
}
