namespace Fanex.NAP.CTS.Library.Criteria.FraudCase.DelayedGoal
{
    using Data.Repository;
    using Fanex.NAP.CTS.Library.Core.BaseClasses;

    public class DelayGoalAssociationAccountInsertCriteria : CommandBase
    {
        public string CustomerAssociationXML { get; set; }

        public override string GetSettingKey()
        {
            return "FS_DelayGoalAccount_ImportFromFPS";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}