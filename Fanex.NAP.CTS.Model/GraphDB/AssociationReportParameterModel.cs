namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System;
    using System.Collections.Generic;

    public class AssociationReportParameterModel
    {
        public DateTime FromAssDate { get; set; }

        public DateTime ToAssDate { get; set; }

        public IEnumerable<int> AccountStatusIDs { get; set; }

        public IEnumerable<int> SubscriberIDs { get; set; }

        public int AssociationStatus { get; set; }

        public IEnumerable<int> EvidenceIDs { get; set; }

        public int AssociationType { get; set; }

        public int TakeRow { get; set; }

        public int SkipRow { get; set; }
    }
}
