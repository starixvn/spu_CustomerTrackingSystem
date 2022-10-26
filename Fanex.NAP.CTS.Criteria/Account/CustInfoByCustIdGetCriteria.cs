namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustInfoByCustIdGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListCustID")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustInfo_GetByCustID";

        public override bool IsValid()
            => true;
    }
}
