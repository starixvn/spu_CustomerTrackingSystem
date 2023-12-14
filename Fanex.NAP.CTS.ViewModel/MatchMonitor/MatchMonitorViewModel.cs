namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.ViewModel.Common;
    using System.Collections.Generic;

    public class MatchMonitorViewModel
    {
        public bool IsHostPermission { get; set; }

        public long CustId { get; set; }
       
        public IEnumerable<ReasonStaticListViewModel> Reasons { get; set; }

        public IEnumerable<SportStaticListViewModel> Sports { get; set; }
       
    }
}