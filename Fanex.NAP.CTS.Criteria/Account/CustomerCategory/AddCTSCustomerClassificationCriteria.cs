using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    public class AddCTSCustomerClassificationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public long CustId { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_CategoryID")]
        public int CategoryId { get; set; }    

        [SpParam(SqlParamName = "ip_Remark")]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
