using Fanex.NAP.CTS.Model.AssociatedAccountMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ViewModel.AssociatedAccountMonitor
{
    public class AssociatedAccountMonitorViewModel
    {
        public long CTSCustId { get; set; }

        public long CustId { get; set; }

        public string Username { get; set; }

        public string Login { get; set; }

        public string AccountStatus { get; set; }

        public string SubscriberName { get; set; }

        public string Remark { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedTime { get; set; }
    }
}
