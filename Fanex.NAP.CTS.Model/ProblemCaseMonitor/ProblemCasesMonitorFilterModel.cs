namespace Fanex.NAP.CTS.Model.ProblemCaseMonitor
{
    using System;

    public class ProblemCasesMonitorFilterModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string ProblemCaseIds { get; set; }

        public string SubscriberIds { get; set; }

        public string ProblemCases { get; set; }

        public string SubscriberName { get; set; }
    }
}
