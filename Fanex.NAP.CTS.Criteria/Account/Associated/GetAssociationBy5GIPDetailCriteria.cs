namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociationBy5GIPDetailCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromCustID", AllowNull = true)]
        public long CustID1 { get; set; }

        [SpParam(SqlParamName = "ip_ToCustID", AllowNull = true)]
        public long CustID2 { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_AIML_GB_5GIP_GetDetails";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
