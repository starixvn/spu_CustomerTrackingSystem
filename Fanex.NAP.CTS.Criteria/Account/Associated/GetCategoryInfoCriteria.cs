namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data.Repository;

    public class GetCategoryInfoCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_Category_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
