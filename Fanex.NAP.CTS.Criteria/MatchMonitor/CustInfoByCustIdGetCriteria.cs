namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CustInfoByCustIdGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListCustID", AllowNull = true)]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustInfo_GetByCustID";

        public override bool IsValid()
            => true;
    }
}
