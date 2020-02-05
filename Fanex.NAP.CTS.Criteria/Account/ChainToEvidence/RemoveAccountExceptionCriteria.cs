using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.Account.ChainToEvidence
{
    public class RemoveAccountExceptionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserID")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "ip_FromCTSCustID")]
        public long FromCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_ToCTSCustID")]
        public long ToCTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_RemoveException";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
