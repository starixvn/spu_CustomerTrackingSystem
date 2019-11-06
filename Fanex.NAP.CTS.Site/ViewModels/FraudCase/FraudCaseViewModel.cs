namespace Fanex.NAP.CTS.Site.ViewModels.FraudCase
{
    using System.Collections.Generic;

    public class FraudCaseViewModel
    {
        public Fanex.NAP.CTS.Model.FraudCase.FraudCase FraudCaseInfo { get; set; }

        public IEnumerable<Fanex.NAP.CTS.Model.DTO.FraudCase> FraudCaseList { get; set; }
    }
}