namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data.Repository;

    public class GetAllEvidenceCriteria : CriteriaBase
    {
        public override string GetSettingKey()
         => "CTS_DC_GetAllEvidences";

        public override bool IsValid()
            => true;
    }
}
