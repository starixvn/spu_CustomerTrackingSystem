namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System.Collections.Generic;

    public class MMDetailResultModel
    {
        public IEnumerable<MMDetailCustModel> Custs { get; set; }

        public IEnumerable<MatchMonitorDetailModel> MatchDetails { get; set; }

        public IEnumerable<MatchMonitorDetailVerifyTransResultModel> VerifiedTrans { get; set; }      
    }
}
