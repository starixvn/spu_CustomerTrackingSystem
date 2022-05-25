namespace Fanex.NAP.CTS.Criteria.CustomerClassification.ProblemAccount
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountUmmarkCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_IsFromQueue")]
        public bool IsFromQueue { get; set; }

        [SpParam(SqlParamName = "ip_ExceptDirectDownline")]
        public bool ExceptDirectDownline { get; set; }      

        [SpParam(SqlParamName = "ip_ProblemAccounts")]
        public string ProblemAccountJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_PA_Remove";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
