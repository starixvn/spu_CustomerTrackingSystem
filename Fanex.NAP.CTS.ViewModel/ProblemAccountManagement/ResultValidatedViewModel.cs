namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{    
    using System.Collections.Generic;
    using System.Linq;

    public class ResultValidatedViewModel
    {
        public ResultValidatedViewModel()
        {
            ProblemAccountInvalidList = Enumerable.Empty<ProblemAccountValidatedViewModel>();
            ProblemAccountDuplicateList = Enumerable.Empty<ProblemAccountValidatedViewModel>();
        }

        public bool IsException { get; set; }

        public IEnumerable<ProblemAccountValidatedViewModel> ProblemAccountInvalidList { get; set; }

        public IEnumerable<ProblemAccountValidatedViewModel> ProblemAccountDuplicateList { get; set; }

    }
}
