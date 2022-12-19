namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System.Collections.Generic;

    public class MatchMonitorStaticListModel
    {
        public IEnumerable<MatchMonitorReasonModel> Reasons { get; set; }

        public IEnumerable<MatchMonitorSportModel> Sports { get; set; }

        public IEnumerable<MatchMonitorBetTypeModel> BetTypes { get; set; }
    }
}
