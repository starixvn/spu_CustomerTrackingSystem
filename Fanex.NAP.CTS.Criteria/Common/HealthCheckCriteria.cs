namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class HealthCheckCriteria : CriteriaBase
    {       
        public override string GetSettingKey()
                => "checkHealth";

        public override bool IsValid()
            => true;
    }
}
