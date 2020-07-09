using Fanex.NAP.CTS.Model.AssociatedAccountMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ViewModel.AssociatedAccountMonitor
{
    public class AssociatedAccountMonitorDetailViewModel
    {
        public AssociatedAccountMonitorDetailViewModel()
        {
            AssociatedAccount = Enumerable.Empty<AssociatedAccountMonitorModel>();
            UserName = Enumerable.Empty<string>();
            Subscriber = Enumerable.Empty<string>();
        }

        public IEnumerable<AssociatedAccountMonitorModel> AssociatedAccount { get; set; }

        public IEnumerable<string> UserName { get; set; }

        public IEnumerable<string> Subscriber { get; set; }

    }
}
