namespace Fanex.NAP.CTS.Model.ProblemMatchMonitor
{
    using System;

    public class MatchDetailInfo
    {
        public DateTime EventDate { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public int HTHomeScore { get; set; }

        public int HTAwayScore { get; set; }

        public int finalhomescore { get; set; }

        public int finalawayscore { get; set; }

        public string FraudCaseName { get; set; }

    }
}