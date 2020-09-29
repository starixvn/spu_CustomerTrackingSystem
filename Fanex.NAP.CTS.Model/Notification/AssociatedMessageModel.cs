namespace Fanex.NAP.CTS.Model.Notification
{
    using System;

    public class AssociatedMessageModel
    {
        public int CTSCustID { get; set; }

        public string UserName { get; set; }

        public int NewAssocitionCount { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public string CreatedDateString
        {
            get
            {
                return CreatedDate == DateTime.MinValue ? "" : CreatedDate.ToString("MMM dd, yyyy HH:mm:ss");
            }
        }

        public int MaxNewAssociationID { get; set; }

        public bool IsSeen { get; set; }
    }
}