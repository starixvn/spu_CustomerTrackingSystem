using System;

namespace Fanex.NAP.CTS.Model.Subscriber
{
    public class SubscriberInfo
    {
        public int SubscriberId { get; set; }

        public string SubscriberName { get; set; }

        public string SubscriberPrefix { get; set; }

        public int SubscriberType { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public bool IsTest { get; set; }

        public bool DCSStatus { get; set; }

        public DateTime DCSIntegrationDate { get; set; }

        public int FPSSubscriberId { get; set; }

        public DateTime TerminatedDate { get; set; }
    }
}
