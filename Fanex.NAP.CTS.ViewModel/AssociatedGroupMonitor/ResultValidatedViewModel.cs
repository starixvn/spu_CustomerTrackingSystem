namespace Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor
{
    using System.Collections.Generic;
    using System.Linq;

    public class ResultValidatedViewModel
    {
        public ResultValidatedViewModel()
        {
            AccountInvalidList = Enumerable.Empty<ValidateUserAssociatedAccountViewModel>();
        }

        public IEnumerable<ValidateUserAssociatedAccountViewModel> AccountInvalidList { get; set; }
    }
}
