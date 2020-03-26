using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.GraphDB
{
    public class GraphAssociationPaging
    {
        public List<GraphAssociationModel> AssociationList { get; set; }

        public int TotalItems { get; set; }
    }
}
