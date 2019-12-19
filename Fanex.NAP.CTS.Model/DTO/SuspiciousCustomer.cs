using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.DTO
{
   public class SuspiciousCustomer
    {
        public bool IsDelinquent { get; set; }

        public bool IsHedge { get; set; }

        public bool IsDirty { get; set; }
    }
}
