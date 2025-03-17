namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System.Collections.Generic;

    public class MMDetailResultModel
    {
        public List<MMDetailCustModel> Custs { get; set; }

        public List<MatchMonitorDetailModel> MatchDetails { get; set; }

        public List<MatchMonitorDetailVerifyTransResultModel> VerifiedTrans { get; set; }

        public MMDetailResultModel()
        {
            Custs = new List<MMDetailCustModel>();
            MatchDetails = new List<MatchMonitorDetailModel>();
            VerifiedTrans = new List<MatchMonitorDetailVerifyTransResultModel>();
        }
    }
}
