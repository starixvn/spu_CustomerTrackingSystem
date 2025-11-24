namespace Fanex.NAP.CTS.Criteria.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountClassificationBySportGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ListCustInfo")]
        public string CustJson { get; set; }

        public int FunctionId { get; set;  }

        public override string GetSettingKey()
        {
            return "CTS_ProblemAccountsClassification_BySport_Daily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
