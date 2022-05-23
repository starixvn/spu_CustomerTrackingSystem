namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ByType")]
        public string ByType { get; set; } // Category or Reason or Evidence

        [SpParam(SqlParamName = "ip_ProblemAccounts")]
        public string ProblemAccountJson { get; set; }       

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_PA_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
