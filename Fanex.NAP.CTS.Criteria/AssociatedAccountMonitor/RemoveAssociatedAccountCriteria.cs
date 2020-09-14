using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.AssociatedAccountMonitor
{
    public class RemoveAssociatedAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public long UserID { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }


        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedAccountMonitor_Remove";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
