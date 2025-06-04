namespace Fanex.NAP.CTS.Criteria.Account.ChainToEvidence
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class AddAccountExceptionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "ip_FromCTSCustID", AllowNull = true)]
        public long FromCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_ToUserName", AllowNull = true)]
        public string ToUserName { get; set; }

        [SpParam(SqlParamName = "ip_Comment", AllowNull = true)]
        public string Comment { get; set; }

        [SpParam(SqlParamName = "op_Error", AllowNull = true)]
        public int ErrorCode { get; set; }

        [SpParam(SqlParamName = "op_ToCTSCustID", AllowNull = true)]
        public UInt64 ToCTSCustId { get; set; }

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
