namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data.Repository;

    public class ValidateUsernameForMainCriteria : CriteriaBase
    {
        public string UsernameList { get; set; }

        public override string GetSettingKey() => "CTS_ValidateUsername";

        public override bool IsValid() => true;
    }
}
