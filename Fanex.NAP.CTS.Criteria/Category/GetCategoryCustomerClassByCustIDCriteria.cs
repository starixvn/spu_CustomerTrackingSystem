namespace Fanex.NAP.CTS.Criteria.Category
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCategoryCustomerClassByCustIDCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs", AllowNull = true)]
        public string CustIDs { get; set; }

        public override string GetSettingKey() => "CTS_DC_CustClassification_GetCategoryByCustIDs";

        public override bool IsValid() => true;
    }
}
