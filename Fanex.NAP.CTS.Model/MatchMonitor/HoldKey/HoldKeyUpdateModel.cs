namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System.Collections.Generic;

    public class HoldKeyUpdateModel
    {
        public int matchId { get; set; }

        public int sporttype { get; set; }

        public bool enabled { get; set; }

        public long userId { get; set; }

        public string userName { get; set; }
    }
}
