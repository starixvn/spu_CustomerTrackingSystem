using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.Account
{
   public class CustomerInfoDetailModel
    {
        public int CTSCustId { get; set; }

        public int CustId { get; set; }

        public string Username { get; set; }

        public string LoginName { get; set; }

        public string Status { get; set; }

        public string AssociationType { get; set; }

        public int SharedDevices { get; set; }

        public int SharedIps { get; set; }

        public DateTime AssociationDate { get; set; }
    }
}
