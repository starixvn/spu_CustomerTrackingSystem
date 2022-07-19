namespace Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor
{
    using System.Collections.Generic;
    using System.Linq;

    public class ResultValidatedViewModel
    {
        public ResultValidatedViewModel()
        {
            AccountInvalidList = Enumerable.Empty<ValidateUserAssociatedAccountViewModel>();
            CustIDs = string.Empty;
            CTSCustIDs = string.Empty;
        }

        public IEnumerable<ValidateUserAssociatedAccountViewModel> AccountInvalidList { get; set; }

        public string CustIDs { get; set; }

        public string CTSCustIDs { get; set; }
    }
}
