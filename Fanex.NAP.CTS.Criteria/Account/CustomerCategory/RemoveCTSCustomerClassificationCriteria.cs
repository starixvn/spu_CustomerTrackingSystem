using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    public class RemoveCTSCustomerClassificationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustId")]
        public long CustId { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_CategoryID")]
        public int CategoryId { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_RemoveCTSCustomerClassification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
