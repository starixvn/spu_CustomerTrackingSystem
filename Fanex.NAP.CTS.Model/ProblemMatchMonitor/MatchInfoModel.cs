namespace Fanex.NAP.CTS.Model.ProblemMatchMonitor
{
    public class MatchInfoModel
    {
        public int MatchId { get; set; }

        public string MatchCode { get; set; }

        public string LeagueName { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public string EventStatus { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

    }
}