namespace Fanex.NAP.CTS.Model.DTO
{
    using System;

    public class Message
    {
        public int CTSCustID { get; set; }

        public string UserName { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public string CreatedDateString
        {
            get
            {
                return CreatedDate == DateTime.MinValue ? "" : CreatedDate.ToString("MMM dd, yyyy H:mm");
            }
        }

        public int NewAssocitionCount { get; set; }
    }
}