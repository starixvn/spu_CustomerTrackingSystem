namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountLicenseeRemoveCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_ProblemAccount_LicenseeRemoveCustomer";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
