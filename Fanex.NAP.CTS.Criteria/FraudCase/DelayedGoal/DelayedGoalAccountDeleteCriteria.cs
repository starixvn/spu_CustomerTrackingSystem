namespace Fanex.NAP.CTS.Criteria.FraudCase.DelayedGoal
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class DelayedGoalAccountDeleteCriteria : CommandBase
    {
        public int CustId { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_DelayGoalAccount_Delete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}