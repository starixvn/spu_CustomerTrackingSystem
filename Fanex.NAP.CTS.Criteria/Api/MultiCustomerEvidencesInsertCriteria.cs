namespace Fanex.NAP.CTS.Criteria.Api
{
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class MultiCustomerEvidencesInsertCriteria : CommandBase
    {
        public string ip_CustIds { get; set; }

        public string ip_EvidenceCodes { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_API_AddEvidencesForCustomers";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(ip_CustIds) & !string.IsNullOrEmpty(ip_EvidenceCodes);
        }
    }
}