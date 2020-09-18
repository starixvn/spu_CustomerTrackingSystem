namespace Fanex.NAP.CTS.Model.ManageSubscriber
{
    using System;

    public class SubscriberModel
    {
        public long SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public string SubscriberPrefix { get; set; }

        public string SubscriberGroupName { get; set; }

        public int RoleMapping { get; set; }

        public int SiteID { get; set; }

        public string SiteName { get; set; }

        public int IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime TerminatedDate { get; set; }

        public int IsTest { get; set; }

        public string CreatedDateString
        {
            get
            {
                return CreatedDate == DateTime.MinValue ? null : CreatedDate.ToString("dd-MMM-yy");
            }
        }

        public string SubscriberNameValidation
        {
            get
            {
                return string.IsNullOrEmpty(SubscriberName) ? null : SubscriberName;
            }
        }

        public string SubscriberPrefixValidation
        {
            get
            {
                return string.IsNullOrEmpty(SubscriberPrefix) ? null : SubscriberPrefix;
            }
        }

        public string TerminatedDateString
        {
            get
            {
                return TerminatedDate == DateTime.MinValue ? "-" : TerminatedDate.ToString("dd-MMM-yy");
            }
        }

    }
}
