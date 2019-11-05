namespace Fanex.NAP.CTS.Criteria.FraudSensorTask
{
    using Fanex.Data.Repository;

    public class CheckBetRuleCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "FS_FraudCase_CheckBet_Rule";

        public override bool IsValid() => true;
    }
}