namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{   
    public class InsertUserFeedbackViewModel
    {
        public long MatchID { get; set; }

        public int SportType { get; set; }

        public bool IsLive { get; set; }

        public int BettypeID { get; set; }

        public string BetID { get; set; }

        public string UserComment { get; set; }

        public int CorrectReasonID { get; set; }

        public string IncorrectReason { get; set; }

        public int UserID { get; set; }
    }
}
