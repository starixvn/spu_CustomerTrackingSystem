namespace Fanex.NAP.CTS.Model.API.ProblemCaseMonitor
{
    using System;

    public class ProblemCaseMonitorParamsModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string SubscriberIDList { get; set; }

        public string ProblemCaseIDList { get; set; }

    }
}
