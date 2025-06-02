namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetCommonCustomerClassCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_Common_CustomerClass_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
