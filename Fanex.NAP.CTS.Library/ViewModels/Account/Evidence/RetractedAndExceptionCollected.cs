namespace Fanex.NAP.CTS.Library.ViewModels.Account.Evidence
{
    using Fanex.NAP.CTS.Library.Models.Account.Evidence;
    using System.Collections.Generic;

    public class RetractedAndExceptionCollected
    {
        public IEnumerable<AccountEvidence> AccountsRetracted { get; set; }
        public IEnumerable<AccountExceptionEvidence> AccountsException { get; set; }
    }
}