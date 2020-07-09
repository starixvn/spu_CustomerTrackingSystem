using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ViewModel.AssociatedAccountMonitor
{
    public class InsertManualAssociatedAccountViewModel
    {
        public InsertManualAssociatedAccountViewModel()
        {
            ResultValidated = new ValidatedAcssociatedAccountViewModel();
        }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public ValidatedAcssociatedAccountViewModel ResultValidated { get; set; }
    }
}
