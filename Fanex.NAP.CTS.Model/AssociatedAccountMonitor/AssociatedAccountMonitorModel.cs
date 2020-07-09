using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.AssociatedAccountMonitor
{
    public class AssociatedAccountMonitorModel
    {
        public long CTSCustId { get; set; }

        public long CustId { get; set; }

        public string Username { get; set; }

        //LoginID
        public string Username2 { get; set; } 

        public string AccountStatus { get; set; }

        public string SubscriberName { get; set; }

        public string Remark { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}