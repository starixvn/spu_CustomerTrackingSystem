namespace Fanex.NAP.CTS.Criteria.SyncCustomerToMainDb
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetLastestEvidenceCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIDList { get; set; }

        [SpParam(SqlParamName = "ip_IsUsingCustID")]
        public bool IsUsingCustID { get; set; }

        [SpParam(SqlParamName = "ip_IsExcludedDetailedCategory")]
        public bool IsExcludedDetailedCategory { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustClassification_GetLatestCategory";

        public override bool IsValid()
            => true;
    }
}
