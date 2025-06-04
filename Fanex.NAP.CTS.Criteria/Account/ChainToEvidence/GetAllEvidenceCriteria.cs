namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data.Repository;

    public class GetAllEvidenceCriteria : CriteriaBase
    {
        public override string GetSettingKey()
         => "CTS_DC_Evidences_GetAll";

        public override bool IsValid()
            => true;
    }
}
