namespace Fanex.NAP.CTS.Criteria.CustomerClassificationAgency.ProblemAccount
{
    using Fanex.Data;
    using Fanex.Data.Repository;
 
    public class ProblemAccountLosingAgencyGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustIds")]
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_ProblemAccountsClassificationAgency_Daily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
