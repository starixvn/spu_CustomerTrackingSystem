namespace Fanex.NAP.CTS.ViewModel.Account.Evidence
{
    using Fanex.NAP.CTS.Model.Account.Evidence;
    using System.Collections.Generic;

    public class RetractedAndExceptionCollected
    {
        public IEnumerable<AccountEvidence> AccountsRetracted { get; set; }
        public IEnumerable<AccountExceptionEvidence> AccountsException { get; set; }
    }
}