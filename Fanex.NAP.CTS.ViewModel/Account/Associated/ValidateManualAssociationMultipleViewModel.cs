namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System.Collections.Generic;
    using System.Linq;

    public class ValidateManualAssociationMultipleViewModel
    {
        public ValidateManualAssociationMultipleViewModel()
        {
            UserNameInvalidList = Enumerable.Empty<string>();
            UserNameDuplicatedList = Enumerable.Empty<string>();
        }

        public IEnumerable<string> UserNameInvalidList { get; set; }

        public IEnumerable<string> UserNameDuplicatedList { get; set; }

    }
}