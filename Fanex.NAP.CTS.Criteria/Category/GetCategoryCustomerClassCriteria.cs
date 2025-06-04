namespace Fanex.NAP.CTS.Criteria.Category
{
    using Fanex.Data.Repository;

    public class GetCategoryCustomerClassCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_Category_Get4CustomerClass";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
