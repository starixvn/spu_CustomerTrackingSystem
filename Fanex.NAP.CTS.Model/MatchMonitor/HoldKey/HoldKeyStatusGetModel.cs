namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class HoldKeyStatusGetModel
    {
        public int matchId { get; set; }

        public bool isHold { get; set; }

        public int userId { get; set; }

        public string userName { get; set; }
    }
}
