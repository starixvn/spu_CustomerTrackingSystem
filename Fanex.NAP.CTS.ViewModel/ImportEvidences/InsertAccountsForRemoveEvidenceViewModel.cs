namespace Fanex.NAP.CTS.ViewModel.ImportEvidences
{
    public class InsertAccountsForRemoveEvidenceViewModel
    {
        public InsertAccountsForRemoveEvidenceViewModel()
        {
            ValidatedResult = new ValidatedAccountsForRemoveEvidenceViewModel();
        }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public ValidatedAccountsForRemoveEvidenceViewModel ValidatedResult { get; set; }
    }
}
