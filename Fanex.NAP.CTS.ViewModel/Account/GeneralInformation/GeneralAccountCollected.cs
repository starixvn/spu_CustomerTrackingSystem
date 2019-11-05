namespace Fanex.NAP.CTS.ViewModel.Account.GeneralInformation
{
    using System.Collections.Generic;

    public class GeneralAccountCollected
    {
        public IEnumerable<CustomerInformation> CustomerInformation { get; set; }

        public IEnumerable<EvidenceByTarget> EvidenceByTargets { get; set; }

        public IEnumerable<AssociatedByAccount> AssociatedByAccounts { get; set; }

        public CustomerFraudSuspicious CustomerFraudSuspicious { get; set; }
    }
}