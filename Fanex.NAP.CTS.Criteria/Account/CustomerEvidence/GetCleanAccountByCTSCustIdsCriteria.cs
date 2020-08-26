namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCleanAccountByCTSCustIdsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustIDList")]
        public string CTSCustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetCleanCustIDsFromCustIDList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
