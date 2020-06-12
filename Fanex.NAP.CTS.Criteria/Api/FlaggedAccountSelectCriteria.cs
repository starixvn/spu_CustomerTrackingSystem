namespace Fanex.NAP.CTS.Criteria.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class FlaggedAccountSelectCriteria : CriteriaBase
    {
        public string ip_CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_API_GetFlaggedCustomers";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(ip_CustIds);
        }
    }
}