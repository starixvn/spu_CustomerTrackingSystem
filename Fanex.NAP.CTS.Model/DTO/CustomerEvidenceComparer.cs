using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.DTO
{
    public class CustomerEvidenceComparer : IEqualityComparer<CustomerEvidence>
    {
        public bool Equals(CustomerEvidence x, CustomerEvidence y)
        {
            return x.Level0CustId ==y.Level0CustId && x.CustId == y.CustId && x.EvidenceCode == y.EvidenceCode;
        }

        public int GetHashCode(CustomerEvidence obj)
        {
            return obj.Level0CustId.GetHashCode() ^ obj.CustId.GetHashCode() ^ obj.EvidenceCode.GetHashCode();
        }
    }
}
