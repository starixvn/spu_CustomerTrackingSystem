using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.AssociatedAccount
{
    public class InsertAssociatedAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs")]
        public string CTSCustIDs { get; set; }

        [SpParam(SqlParamName = "ip_Remark")]
        public string remark { get; set; }

        [SpParam(SqlParamName = "ip_UserID")]
        public int userID { get; set; }
        
        public override string GetSettingKey()
            => "CTS_DC_InsertAssociatedAccountMonitor";

        public override bool IsValid()
            => true;
    }
}