namespace Fanex.NAP.CTS.Criteria.Account.ChainToEvidence
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AddAccountExceptionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "ip_FromCTSCustID")]
        public long FromCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_ToUserName")]
        public string ToUserName { get; set; }

        [SpParam(SqlParamName = "ip_Comment")]
        public string Comment { get; set; }

        [SpParam(SqlParamName = "op_Error")]
        public int ErrorCode { get; set; }

        [SpParam(SqlParamName = "op_ToCTSCustID")]
        public int ToCTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_InsertException";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
