using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ViewModel.FeedDataToSoon88
{
    public class PreparedUpdateDataToSoon88ViewModel
    {
        public string action
        {
            get { return "block"; }
        }

        public string reason { get; set; }

        public string account { get; set; }

        public object association { get; set; }
    }
}
