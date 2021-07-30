namespace Fanex.NAP.CTS.ViewModel.Common
{
    using System.Collections.Generic;
    using System.Linq;

    public class ValidatedAccountsViewModel
    {
        public ValidatedAccountsViewModel()
        {
            UserNameInvalidList = Enumerable.Empty<string>();
            UserNameDuplicatedList = Enumerable.Empty<string>();
            UserNameValidList = Enumerable.Empty<string>();

        }

        public bool IsException { get; set; }

        public IEnumerable<string> UserNameInvalidList { get; set; }

        public IEnumerable<string> UserNameDuplicatedList { get; set; }

        public IEnumerable<string> UserNameValidList { get; set; }
    }
}
