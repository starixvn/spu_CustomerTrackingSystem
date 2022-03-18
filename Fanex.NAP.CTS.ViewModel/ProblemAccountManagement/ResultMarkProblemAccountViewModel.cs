namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{
    public class ResultMarkProblemAccountViewModel
    {
        public ResultMarkProblemAccountViewModel()
        {
            ResultValidated = new ResultValidatedViewModel();
        }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public ResultValidatedViewModel ResultValidated { get; set; }
    }
}
