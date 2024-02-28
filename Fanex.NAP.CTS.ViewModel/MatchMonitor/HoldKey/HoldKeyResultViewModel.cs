namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{   
    public class HoldKeyResultViewModel
    {
        public HoldKeyResultViewModel(HoldKeyStatusGetViewModel data)
        {
            IsSuccess = false;
            HoldKeyStatus = data;
            Message = string.Empty;
        }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public HoldKeyStatusGetViewModel HoldKeyStatus { get; set; }
    }
}
