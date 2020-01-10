using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.Account
{
    public class GraphAssociationModel
    {
        public int? CustId { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public int AssociationType { get; set; }

        public string AssociationTypeName
        {
            get
            {
                if (this.AssociationType == 0)
                {
                    return "Divice, IP";
                }

                return Enum.GetName(typeof(AssociationType), AssociationType);
            }
        }

        public DateTime AssociationDate { get; set; }

        public int SharedDevices { get; set; }

        public int SharedIPs { get; set; }

        public string StatusName { get; set; }

        public int StatusId { get; set; }
    }
}
