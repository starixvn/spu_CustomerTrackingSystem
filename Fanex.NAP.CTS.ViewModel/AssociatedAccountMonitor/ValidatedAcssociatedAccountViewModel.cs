using System.Collections.Generic;
using System.Linq;


namespace Fanex.NAP.CTS.ViewModel
{
    public class ValidatedAcssociatedAccountViewModel
    {
        public ValidatedAcssociatedAccountViewModel()
        {
            UserNameInvalidList = Enumerable.Empty<string>();
            UserNameDuplicatedList = Enumerable.Empty<string>();
        }

        public bool IsException { get; set; }

        public string ValideCTSCustId { get; set; }

        public IEnumerable<string> UserNameInvalidList { get; set; }

        public IEnumerable<string> UserNameDuplicatedList { get; set; }

    }
}
