namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System;

    public class AssociationReportParameterModel
    {
        public DateTime AssociatedDateFrom { get; set; }

        public DateTime AssociatedDateTo { get; set; }

        public string AccountStatuses { get; set; }

        public string SubscriberIDs { get; set; }

        public int AssociationStatus { get; set; }

        public string EvidenceIDs { get; set; }

        public int AssociationType { get; set; }

        public int TakeRow { get; set; }

        public int SkipRow { get; set; }
    }
}
