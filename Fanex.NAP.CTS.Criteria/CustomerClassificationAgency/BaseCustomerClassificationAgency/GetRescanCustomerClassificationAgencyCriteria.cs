namespace Fanex.NAP.CTS.Criteria.CustomerClassificationAgency.BaseCustomerClassificationAgency
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetRescanCustomerClassificationAgencyCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs", AllowNull = true)]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassificationAgency_Rescan_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
