using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.Account
{
   public class CustomerAssociationByAccountDetailModel
    {
        public int Tag { get; set; }

        public int CTSCustId { get; set; }

        public int CustId { get; set; }

        public string Username { get; set; }

        public string LoginId { get; set; }

        public string Status { get; set; }

        public int StatusId { get; set; }

        public string Subscriber { get; set; }

        public int AssociationType { get; set; }

        public string AssociationTypeName { get; set; }

        public int SharedDevices { get; set; }

        public int SharedIps { get; set; }

        public DateTime AssociationDate { get; set; }
    }
}
