using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.Account
{
   public class CustomerInfoByCTSCustIdModel
    {
        public DateTime LastLoginTime { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public int CustId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
