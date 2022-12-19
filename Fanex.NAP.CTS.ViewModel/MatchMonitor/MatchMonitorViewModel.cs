namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using System.Collections.Generic;

    public class MatchMonitorViewModel
    {
        public bool IsHostPermission { get; set; }

        public long CustId { get; set; }
       
        public IEnumerable<MatchMonitorReasonModel> Reasons { get; set; }

        public IEnumerable<MatchMonitorSportModel> Sports { get; set; }
       
    }
}