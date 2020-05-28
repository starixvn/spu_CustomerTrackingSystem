namespace Fanex.NAP.CTS.Model.ProblemAccountMonitor
{
    using System.Collections.Generic;

    public class ProblemAccountMonitorModel
    {
        public IEnumerable<SuspiciousTicketInfo> SuspiciousTicketInfos { get; set; }

        public IEnumerable<MatchInfo> MatchInfos { get; set; }

    }
}