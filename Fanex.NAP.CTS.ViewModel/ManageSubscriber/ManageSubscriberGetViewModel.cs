using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ViewModel.ManageSubscriber
{
    public class ManageSubscriberGetViewModel
    {
        public long SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public string SubscriberPrefix { get; set; }

        public int RoleMapping { get; set; }

        public int SiteID { get; set; }

        public string SiteName { get; set; }

        public int IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime TerminatedDate { get; set; }
    }
}
