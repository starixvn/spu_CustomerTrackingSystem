namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountMarkCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputType")]
        public string InputType { get; set; } // Category or Reason or Evidence

        [SpParam(SqlParamName = "ip_IsFromQueue")]
        public bool IsFromQueue { get; set; }

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
