namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustEvidenceByReasonIdCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_ProblemAccountManagement_GetCategoryFilter";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
