namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ValidateManualAssociationMultipleCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_RootCTSCustID")]
        public long FromCTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_UsernameList")]
        public string ToUserNameList { get; set; }        

        public override string GetSettingKey()
            => "CTS_DC_AddManualAssociationMultipleValidation";

        public override bool IsValid()
            => true;

    }
}
