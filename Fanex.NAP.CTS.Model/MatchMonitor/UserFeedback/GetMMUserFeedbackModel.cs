namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;

    public class GetMMUserFeedbackModel
    {
        public int MMUserCommentID { get; set; }

        public string UserComment { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string CorrectReason { get; set; }

        public string IncorrectReason { get; set; }

        public string FromTransInfo { get; set; }

        public string ToTransInfo { get; set; }
    }
}
