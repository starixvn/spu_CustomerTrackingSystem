namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System.Collections.Generic;

    public class MatchMonitorDetailResultModel
    {
        public IEnumerable<MatchMonitorDetailModel> MatchDetails { get; set; }

        public IEnumerable<MatchMonitorVerifiedTransDetailModel> VerifiedTrans { get; set; }      
    }
}
