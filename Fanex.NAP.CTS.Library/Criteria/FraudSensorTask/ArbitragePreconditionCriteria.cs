namespace Fanex.NAP.CTS.Library.Criteria.FraudSensorTask
{
    using Fanex.Data.Repository;

    public class ArbitragePreconditionCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "FS_FraudCase_Arbitrage_PreCondition";

        public override bool IsValid() => true;
    }
}