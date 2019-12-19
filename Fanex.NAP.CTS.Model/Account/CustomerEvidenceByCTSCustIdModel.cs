using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.Account
{
   public class CustomerEvidenceByCTSCustIdModel
    {
        public string EvidenceName { get; set; }

        public string EvidenceDesc { get; set; }

        public string EvidenceCode { get; set; }

        public int OrderNo { get; set; }

        public DateTime CreatedDate { get; set; }

        public string EvidenceGroupName { get; set; }
    }
}
