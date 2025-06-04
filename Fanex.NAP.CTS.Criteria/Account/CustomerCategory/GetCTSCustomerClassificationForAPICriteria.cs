namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCTSCustomerClassificationForAPICriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs", AllowNull = true)]
        public string CustIdList { get; set; }

        [SpParam(SqlParamName = "ip_ParentCateIDs", AllowNull = true)]
        public string ParentCateIdList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Get4API";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
