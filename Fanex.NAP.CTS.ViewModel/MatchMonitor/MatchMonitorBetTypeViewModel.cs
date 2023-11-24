namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    public class MatchMonitorBetTypeViewModel
    {        
        public int BetTypeID { get; set; }

        public int BetID { get; set; }

        public string BetTypeName { get; set; }

        public int SportID { get; set; }

        public bool IsSupportSuspicious { get; set; }

        public bool IsSelected { get; set; }
    }
}
