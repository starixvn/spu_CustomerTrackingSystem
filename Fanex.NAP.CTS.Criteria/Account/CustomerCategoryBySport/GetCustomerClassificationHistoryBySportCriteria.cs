namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategoryBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;  

    public class GetCustomerClassificationHistoryBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public long CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_SportID", AllowNull = true)]
        public int SportID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_History_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
