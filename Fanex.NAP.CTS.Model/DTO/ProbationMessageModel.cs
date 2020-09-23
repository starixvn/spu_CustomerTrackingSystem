namespace Fanex.NAP.CTS.Model.DTO
{
    using System;

    public class ProbationMessageModel
    {
        public int NotificationType { get; set; }

        public int TotalScanned { get; set; }

        public int TotalFailed { get; set; } 

        public int TotalGeneralFailed { get; set; }

        public DateTime CreatedDate { get; set; }

        public long MaxNotificationID { get; set; }

        public bool IsSeen { get; set; }

        public string CreatedDateString
        {
            get
            {
                return CreatedDate == DateTime.MinValue ? "" : CreatedDate.ToString("dd-MMM-yyyy");
            }
        }
    }
}