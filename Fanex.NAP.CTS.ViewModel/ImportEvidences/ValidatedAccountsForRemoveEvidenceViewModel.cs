namespace Fanex.NAP.CTS.ViewModel.ImportEvidences
{
    using System.Collections.Generic;
    using System.Linq;

    public class ValidatedAccountsForRemoveEvidenceViewModel
    {
        public ValidatedAccountsForRemoveEvidenceViewModel()
        {
            UserNameInvalidList = Enumerable.Empty<string>();
            UserNameDuplicatedList = Enumerable.Empty<string>();
        }

        public bool IsException { get; set; }

        public IEnumerable<string> UserNameInvalidList { get; set; }

        public IEnumerable<string> UserNameDuplicatedList { get; set; }
    }
}
