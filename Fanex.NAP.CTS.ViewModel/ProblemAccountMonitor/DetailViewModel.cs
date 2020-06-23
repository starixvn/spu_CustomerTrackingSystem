namespace Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor
{
    public class DetailViewModel
    {
        public int MatchId { get; set; }

        public int FraudCaseId { get; set; }

        public string SiteIds { get; set; }

        public bool IsHostPermission { get; set; }

        public VoidRequestToTVSParam VoidRequestToTVSParam { get; set; }

    }
}