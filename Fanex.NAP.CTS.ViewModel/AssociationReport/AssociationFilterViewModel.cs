namespace Fanex.NAP.CTS.ViewModel.AssociationReport
{
    using System;

    public class AssociationFilterViewModel
    {
        public int draw { get; set; }

        public int start { get; set; }

        public int length { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string AccountStatusIds { get; set; }

        public string SiteName { get; set; }

        public string SubscriberIDs { get; set; }

        public int AssociationStatus { get; set; }

        public string EvidenceIDs { get; set; }

        public string EvidenceCode { get; set; }

        public int AssociationType { get; set; }        
    }
}