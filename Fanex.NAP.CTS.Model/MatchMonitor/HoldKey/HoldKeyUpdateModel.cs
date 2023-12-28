namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System.Collections.Generic;

    public class HoldKeyUpdateModel
    {
        public IEnumerable<string> matchIds { get; set; }

        public bool enabled { get; set; }

        public long userId { get; set; }
    }
}
