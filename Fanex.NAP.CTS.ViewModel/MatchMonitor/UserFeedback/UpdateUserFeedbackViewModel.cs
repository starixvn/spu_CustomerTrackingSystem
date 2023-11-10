namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    public class UpdateUserFeedbackViewModel
    {
        public int UserID { get; set; }

        public long MMUserCommentID { get; set; }

        public string UserComment { get; set; }

        public int CorrectReasonID { get; set; }

        public string IncorrectReason { get; set; }
    }
}
