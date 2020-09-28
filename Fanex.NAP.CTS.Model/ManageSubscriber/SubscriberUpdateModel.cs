namespace Fanex.NAP.CTS.Model.ManageSubscriber
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SubscriberUpdateModel
    {
        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public int SubscriberType { get; set; }

        public string IBCUserNamePrefix { get; set; }

        public int IsTest { get; set; }

        public string SiteListJson { get; set; }
        
    }
}
