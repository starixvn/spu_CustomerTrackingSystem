namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountLicenseeRemoveCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "CustomerList")]
        public string CustomerList { get; set; }

        [SpParam(SqlParamName = "UserID")]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "IP")]
        public string IP { get; set; }

        [SpParam(SqlParamName = "SuccessAllDownlineList")]
        public string SuccessAllDownlineList { get; set; }

        [SpParam(SqlParamName = "ErrCode")]
        public int ErrCode { get; set; }

        [SpParam(SqlParamName = "ErrMsg")]
        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_ProblemAccount_LicenseeRemoveCustomer";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
