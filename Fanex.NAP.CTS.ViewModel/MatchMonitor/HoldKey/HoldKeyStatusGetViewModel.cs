namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{   
    public class HoldKeyStatusGetViewModel
    {
        public HoldKeyStatusGetViewModel()
        {
            IsHold = false;
        }

        public bool IsHold { get; set; }

        public string UserName { get; set; }
    }
}
