namespace Fanex.NAP.CTS.Library.Criteria.FraudSensorTask
{
    using Data.Repository;

    public class DelayGoalPreconditionCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_FraudCase_DelayGoal_PreCondition";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}