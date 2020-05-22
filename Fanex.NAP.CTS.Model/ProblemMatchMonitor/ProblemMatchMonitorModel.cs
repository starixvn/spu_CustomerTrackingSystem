namespace Fanex.NAP.CTS.Model.ProblemMatchMonitor
{
    using System.Collections.Generic;

    public class ProblemMatchMonitorModel
    {
        public IEnumerable<SuspiciousTicketInfo> SuspiciousTicketInfos { get; set; }

        public IEnumerable<MatchInfo> MatchInfos { get; set; }

    }
}