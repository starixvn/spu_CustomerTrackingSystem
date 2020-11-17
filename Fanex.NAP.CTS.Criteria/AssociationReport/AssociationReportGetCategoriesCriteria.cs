namespace Fanex.NAP.CTS.Criteria.AssociationReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AssociationReportGetCategoriesCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustIDList")]
        public string CTSCustIdList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Report_GetAssociatedAccount";

        public override bool IsValid()
            => true;
    }
}
