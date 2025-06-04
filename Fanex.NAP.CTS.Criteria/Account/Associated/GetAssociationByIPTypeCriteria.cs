namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociationByIPTypeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromCustID", AllowNull = true)]
        public long CustID1 { get; set; }

        [SpParam(SqlParamName = "ip_ToCustID", AllowNull = true)]
        public long CustID2 { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociationByIP_GetAssType";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
