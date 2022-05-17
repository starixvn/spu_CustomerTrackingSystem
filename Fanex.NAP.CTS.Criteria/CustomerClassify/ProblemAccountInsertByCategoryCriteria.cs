namespace Fanex.NAP.CTS.Criteria.CustomerClassify
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountInsertByCategoryCriteria : NonQueryCommand
    {

        [SpParam(SqlParamName = "ip_ProblemAccounts")]
        public string ProblemAccountJson { get; set; }

        [SpParam(SqlParamName = "ip_IsRescan")]
        public bool IsRescan { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_ProblemAccount_InsertByCategory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
