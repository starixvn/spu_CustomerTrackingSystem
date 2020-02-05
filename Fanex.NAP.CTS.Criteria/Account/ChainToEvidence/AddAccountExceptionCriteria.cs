using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.CTS.Criteria.Account.ChainToEvidence
{
    public class AddAccountExceptionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "op_Error")]
        public int ErrorCode { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "ip_FromCTSCustID")]
        public long FromCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_ToUserName")]
        public string ToUserName { get; set; }

        [SpParam(SqlParamName = "ip_Comment")]
        public string Comment { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AddException";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
