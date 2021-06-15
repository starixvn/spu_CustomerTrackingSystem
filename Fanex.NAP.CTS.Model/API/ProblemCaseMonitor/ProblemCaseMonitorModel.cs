namespace Fanex.NAP.CTS.Model.API.ProblemCaseMonitor
{
    public class ProblemCaseMonitorModel
    {
        public string UserName { get; set; }

        public string Subscriber { get; set; }

        public bool IsRobot { get; set; }

        public bool IsHedging { get; set; }

        public bool IsGroupBetting { get; set; }

        public bool IsSubspiciousAssociation { get; set; }

        public bool IsTimeInterval { get; set; }

        public bool IsContinuousBetting { get; set; }

        public bool IsOddsFavor { get; set; }

        public bool IsHighOddsBettor { get; set; }

        public bool IsProPunter { get; set; }

    }
}
