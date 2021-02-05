using Fanex.NAP.CTS.ViewModel.Common;

namespace Fanex.NAP.CTS.ViewModel.ImportEvidences
{
    public class InsertAccountsForRemoveEvidenceViewModel
    {
        public InsertAccountsForRemoveEvidenceViewModel()
        {
            ValidatedResult = new ValidatedAccountsViewModel();
        }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public ValidatedAccountsViewModel ValidatedResult { get; set; }
    }
}
