using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.Account
{
    public class GraphCustomerModel
    {
        public int? CustId { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public string AssociationType { get; set; }

        public DateTime AssociationDate { get; set; }

        public int SharedDevices { get; set; }

        public int SharedIPs { get; set; }

        public string StatusName { get; set; }

        public int StatusId { get; set; }
    }
}
