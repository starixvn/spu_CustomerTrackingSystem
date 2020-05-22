namespace Fanex.NAP.CTS.Model.ProblemMatchMonitor
{
    public class SuspiciousTicketInfo
    {
        public int MatchId { get; set; }

        public int FraudCaseId { get; set; }

        public string FraudCaseName { get; set; }

        public int SuspiciousTicketsCount { get; set; }

    }
}