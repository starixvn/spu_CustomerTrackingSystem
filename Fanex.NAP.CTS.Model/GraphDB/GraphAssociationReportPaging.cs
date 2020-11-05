namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System.Collections.Generic;

    public class GraphAssociationReportPaging
    {
        public List<AssociationReportModel> AssociationList { get; set; }

        public int TotalItems { get; set; }
    }
}
