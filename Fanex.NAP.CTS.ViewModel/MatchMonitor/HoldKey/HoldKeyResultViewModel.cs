namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{   
    public class HoldKeyResultViewModel
    {
        public HoldKeyResultViewModel()
        {
            IsSuccess = true;
        }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public HoldKeyStatusGetViewModel HoldKeyStatus { get; set; }
    }
}
