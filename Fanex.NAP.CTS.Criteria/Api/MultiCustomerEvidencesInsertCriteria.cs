namespace Fanex.NAP.CTS.Criteria.Api
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MultiCustomerEvidencesInsertCriteria : CriteriaBase
    {
        public string ip_CustIds { get; set; }

        public string ip_EvidenceCodes { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public Int64 ip_CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidence_InsertMultilpleEvidencesForCustomers";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(ip_CustIds) & !string.IsNullOrEmpty(ip_EvidenceCodes);
        }
    }
}