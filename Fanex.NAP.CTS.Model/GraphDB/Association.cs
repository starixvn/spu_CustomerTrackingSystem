namespace Fanex.NAP.CTS.Model.GraphDB
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [XName("A")]
    public class Association
    {
        public int CustId { get; set; }

        public DateTime CreatedDate { get; set; }

        [XName("AssociatedDate")]
        public DateTime FirstAssociatedDate { get; set; }
    }
}
