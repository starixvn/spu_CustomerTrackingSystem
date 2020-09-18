using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.ManageSubscriber
{
    [Serializable]
    public class SubscriberInsertModel
    {
        public int SiteID { get; set; }

        public string Site { get; set; }

        public string IBCUserNamePrefix { get; set; }

        public string SubscriberType { get; set; }

        public string SubscriberName { get; set; }

        public int RoleMapping { get; set; }

        public int IsTest { get; set; }

        public int SubscriberID { get; set; }
      
    }
}
