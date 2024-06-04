namespace Fanex.NAP.CTS.Criteria.Api.TW
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertTWInitialSmartCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfoJson { get; set; }       

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_InitialSmart_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
