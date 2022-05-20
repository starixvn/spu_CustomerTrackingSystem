namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{    
    using System.Collections.Generic;
    using System.Linq;

    public class ResultValidatedViewModel
    {
        public ResultValidatedViewModel()
        {
            IsException = false;
            ProblemAccountInvalidList = Enumerable.Empty<ProblemAccountValidatedViewModel>();
        }

        public bool IsException { get; set; }

        public IEnumerable<ProblemAccountValidatedViewModel> ProblemAccountInvalidList { get; set; }

        public bool IsCreditSite { get; set; }
    }
}
