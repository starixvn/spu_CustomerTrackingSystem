namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountLicenseeUpdateCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "CustomerList")]
        public string CustomerList { get; set; }

        [SpParam(SqlParamName = "ReasonId")]
        public int ReasonID { get; set; }

        [SpParam(SqlParamName = "ActionId")]
        public int ActionId { get; set; }

        [SpParam(SqlParamName = "Comment")]
        public string Comment { get; set; }

        [SpParam(SqlParamName = "VoidTime")]
        public string VoidTime { get; set; }

        [SpParam(SqlParamName = "LimitAmount")]
        public decimal LimitAmount { get; set; }

        [SpParam(SqlParamName = "UpdateTime")]
        public bool UpdateTime { get; set; }

        [SpParam(SqlParamName = "UpdateLimitAmount")]
        public bool UpdateLimitAmount { get; set; }

        [SpParam(SqlParamName = "UserID")]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "IP")]
        public string IP { get; set; }

        [SpParam(SqlParamName = "ErrCode")]
        public int ErrCode { get; set; }

        [SpParam(SqlParamName = "ErrMsg")]
        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_ProblemAccount_LicenseeAccountReasonUpd";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
