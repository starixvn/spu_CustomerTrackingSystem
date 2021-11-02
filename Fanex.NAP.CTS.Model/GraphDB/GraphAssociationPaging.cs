namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System.Collections.Generic;

    public class GraphAssociationPaging
    {
        public GraphAssociationPaging()
        {
            AssociationList = new List<GraphAssociationModel>();
        }

        public IEnumerable<GraphAssociationModel> AssociationList { get; set; }

        public int TotalItems { get; set; }
    }
}
