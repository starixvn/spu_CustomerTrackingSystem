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
            ProblemAccountDuplicateList = Enumerable.Empty<ProblemAccountValidatedViewModel>();
        }

        public bool IsException { get; set; }

        public IEnumerable<ProblemAccountValidatedViewModel> ProblemAccountInvalidList { get; set; }

        public IEnumerable<ProblemAccountValidatedViewModel> ProblemAccountDuplicateList { get; set; }

        public int CategoryID { get; set; }

        public string Comment { get; set; }

        public bool IsCreditSite { get; set; }
    }
}
