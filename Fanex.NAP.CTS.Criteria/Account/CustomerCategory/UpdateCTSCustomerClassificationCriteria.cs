using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    public class UpdateCTSCustomerClassificationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustId")]
        public long CustId { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_CategoryID")]
        public int CategoryId { get; set; }

        [SpParam(SqlParamName = "ip_SportGroupIDList")]
        public string SportGroupIds { get; set; }

        [SpParam(SqlParamName = "ip_Remark")]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_UpdateCTSCustomerClassification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
