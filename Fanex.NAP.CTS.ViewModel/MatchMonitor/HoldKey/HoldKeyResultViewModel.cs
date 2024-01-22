namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{   
    public class HoldKeyResultViewModel
    {
        public HoldKeyResultViewModel()
        {
            IsSuccess = false;
            HoldKeyStatus = new HoldKeyStatusGetViewModel { IsHold = false };
            Message = string.Empty;
        }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public HoldKeyStatusGetViewModel HoldKeyStatus { get; set; }
    }
}
