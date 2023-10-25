namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System;

    public class GetMMUserFeedbackViewModel
    {
        public int MMUserCommentID { get; set; }

        public string UserComment { get; set; }

        public string LastModifiedBy { get; set; }

        public string LastModifiedDate { get; set; }

        public string CorrectReason { get; set; }

        public string IncorrectReason { get; set; }

        public string IncorrectReasonRaw { get; set; }

        public string FromTransInfo { get; set; }

        public string ToTransInfo { get; set; }
    }
}
