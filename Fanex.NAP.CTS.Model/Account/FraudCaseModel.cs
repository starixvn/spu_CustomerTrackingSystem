namespace Fanex.NAP.CTS.Model.Account
{
    public class FraudCaseModel
    {
        public int FraudCaseId { get; set; }

        public string FraudCaseName { get; set; }

        public string FraudCaseDesc { get; set; }

        public int FraudCaseInterval { get; set; }

        public bool IsActive { get; set; }

        public bool IsTest { get; set; }

    }
}
