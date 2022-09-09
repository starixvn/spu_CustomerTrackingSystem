using Fanex.NAP.CTS.Core.Helpers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.GraphDB
{
    [XName("Association")]
    public class AssociationForExtAcc
    {
        public long CustId { get; set; }

        public int CustSubId { get; set; }

        [XName("Date")]
        public string AssociationDate { get; set; }
    }
}
