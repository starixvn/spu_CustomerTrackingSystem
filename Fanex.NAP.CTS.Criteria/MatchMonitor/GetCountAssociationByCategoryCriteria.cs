namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCountAssociationByCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListCustID")]
        public string CustIDList { get; set; }

        [SpParam(SqlParamName = "ip_ListCategoryID")]
        public string CategoryIDList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Association_CountByCategoryID";

        public override bool IsValid()
            => true;
    }
}
