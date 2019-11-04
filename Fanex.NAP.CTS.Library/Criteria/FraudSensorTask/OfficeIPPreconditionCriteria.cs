namespace Fanex.NAP.CTS.Library.Criteria.FraudSensorTask
{
    using Fanex.Data.Repository;

    public class OfficeIPPreconditionCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "FS_FraudCase_OfficeIP_PreCondition";

        public override bool IsValid() => true;
    }
}