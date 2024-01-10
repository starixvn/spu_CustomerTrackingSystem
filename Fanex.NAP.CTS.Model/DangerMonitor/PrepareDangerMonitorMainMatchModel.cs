namespace Fanex.NAP.CTS.Model.DangerMonitor
{
    using Fanex.NAP.CTS.Model.Common;
    using System.Collections.Generic;

    public class PrepareDangerMonitorMainMatchModel
    {
        public GetDangerMonitorGenerateModel GetDangerMonitorGenerate { get; set; }

        public IEnumerable<MainMatchModel> MatchSectionMainMatch { get; set; }
    }
}
