namespace Fanex.NAP.CTS.Criteria.FraudCase.DelayedGoal
{  
    using Fanex.NAP.CTS.Core.BaseClasses;

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