namespace Fanex.NAP.CTS.Model.Account
{
    using System.Collections.Generic;

    public class CustomerProfileFraudInfoModel
    {
        public SuspiciousCustomerModel SuspiciouseCustomer { get; set; }

        public List<FraudCaseModel> FraudCaseList { get; set; }
    }
}
