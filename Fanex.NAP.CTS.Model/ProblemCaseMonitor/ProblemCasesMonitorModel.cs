namespace Fanex.NAP.CTS.Model.ProblemCaseMonitor
{
    using System;

    public class ProblemCasesMonitorModel
    {
        public Int64 CTSCustId { get; set; }

        public string UserName { get; set; }

        public string Subscriber { get; set; }

        public bool Robot { get; set; }

        public bool SuspiciousAssociation { get; set; }

        public bool ProPunter { get; set; }
    }
}
