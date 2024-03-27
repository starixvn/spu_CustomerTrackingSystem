namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{   
    public class HoldKeyUpdateViewModel
    {
        public int MatchId { get; set; }

        public int SportType { get; set; }

        public int MatchSection { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public bool IsHold { get; set; }        

        public bool IsCurrentStatus { get; set; }
    }
}
