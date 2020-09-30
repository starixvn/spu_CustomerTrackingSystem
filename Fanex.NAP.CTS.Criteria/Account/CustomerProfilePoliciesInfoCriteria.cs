namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data.Repository;

    public class CustomerProfilePoliciesInfoCriteria : CriteriaBase
    {
        public long CustID { get; set; }

        public override string GetSettingKey()
        {
            return "BLC_SettingSelect_ByCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
