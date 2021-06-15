namespace Fanex.NAP.CTS.ViewModel.ProblemCaseMonitor
{
    using Fanex.NAP.CTS.Model.ProblemCaseMonitor;
    using Fanex.NAP.CTS.ViewModel.Common;
    using System.Collections.Generic;

    public class ProblemCaseViewModel
    {
        public bool IsHostPermission { get; set; }

        public long CustId { get; set; }

        public List<ProblemCasesFilterModel> ProblemCases { get; set; }

        public List<SubscriberGroupViewModel> Subscriber { get; set; }
    }
}
