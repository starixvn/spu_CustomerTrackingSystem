namespace Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor
{
    public class ResultSaveAssociatedAccountViewModel
    {
        public ResultSaveAssociatedAccountViewModel()
        {
            ResultValidated = new ResultValidatedViewModel();
            ErrorCode = 0;
            ErrorMessage = string.Empty;
        }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public ResultValidatedViewModel ResultValidated { get; set; }
    }
}
