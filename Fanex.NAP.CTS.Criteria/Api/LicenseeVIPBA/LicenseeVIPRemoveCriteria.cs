namespace Fanex.NAP.CTS.Criteria.Api
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class LicenseeVIPRemoveCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustJson")]
        public string CustJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_LicVIP_Remove";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
