using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ViewModel.Account.TransactionHistory
{
   public class TransactionHistoryViewModel
    {
        public string TransId { get; set; }

        public string TransTime { get; set; }

        public string DeviceId { get; set; }

        public string Ip { get; set; }

        public long IpId { get; set; }

        public string Flagged { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string OSName { get; set; }

        public string BrowserName { get; set; }

        public string Url { get; set; }
    }
}
