using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.GraphDB
{
    public class Relationship
    {
        public int StartNodeId { get; set; }

        public int EndNodeId { get; set; }

        public string Type { get; set; }
    }
}
