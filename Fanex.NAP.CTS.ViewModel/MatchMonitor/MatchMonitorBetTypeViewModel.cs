namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    public class MatchMonitorBetTypeViewModel
    {        
        public int BetTypeID { get; set; }

        public int BetID { get; set; }

        public string BetTypeName { get; set; }

        public int SportID { get; set; }

        public int BetChoiceType { get; set; }

        public string BetIDPattern { get; set; }

        public bool IsSupportSuspicious { get; set; }

        public bool IsSelected { get; set; }

        public string NameDetailsX { get; set; }
    }
}
