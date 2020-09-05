using Fanex.NAP.CTS.Core.Helpers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.CustomerClass
{
    [XName("r")]
    public class CustomerClassForMainDb
    {
        [XName("id")]
        public int CustID { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }
    }
}
