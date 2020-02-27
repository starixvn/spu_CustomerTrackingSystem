using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Account
{
    public class CustomerRobotUserStatusCriteria : CriteriaBase
    {
        public long CustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_RobotUserStatus_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
