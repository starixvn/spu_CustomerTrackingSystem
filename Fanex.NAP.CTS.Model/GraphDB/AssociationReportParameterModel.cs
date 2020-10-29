namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System;

    public class AssociationReportParameterModel
    {
        public DateTime AssociatedDateFrom { get; set; }

        public DateTime AssociatedDateTo { get; set; }

        public int AccountStatus { get; set; }

        public string SiteName { get; set; }

        public int AssociationStatus { get; set; }

        public string EvidenceCode { get; set; }

        public int AssociationType { get; set; }
    }
}
