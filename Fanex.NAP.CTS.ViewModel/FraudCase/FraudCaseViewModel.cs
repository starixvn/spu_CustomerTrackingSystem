namespace Fanex.NAP.CTS.ViewModel.FraudCase
{
    using Fanex.NAP.CTS.Model.FraudCase;

    using System.Collections.Generic;

    public class FraudCaseViewModel
    {
        public FraudCase FraudCaseInfo { get; set; }

        public IEnumerable<Fanex.NAP.CTS.Model.DTO.FraudCase> FraudCaseList { get; set; }
    }
}