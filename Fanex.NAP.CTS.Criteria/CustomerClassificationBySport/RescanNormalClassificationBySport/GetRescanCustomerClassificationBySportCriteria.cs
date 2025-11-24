namespace Fanex.NAP.CTS.Criteria.CustomerClassificationBySport.RescanNormalClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetRescanCustomerClassificationBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustInfoList", AllowNull = true)]
        public string CustInfoJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_Rescan_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
