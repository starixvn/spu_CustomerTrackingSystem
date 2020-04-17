using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    public class AddManualAssociationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromSubscriberID")]
        public int FromSubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_FromCTSCustID")]
        public long FromCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_ToSubscriberID")]
        public int ToSubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_ToCTSCustID")]
        public long ToCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_Remark")]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AddManualAssociation";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
