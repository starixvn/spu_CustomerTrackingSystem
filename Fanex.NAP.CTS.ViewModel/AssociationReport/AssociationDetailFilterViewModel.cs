namespace Fanex.NAP.CTS.ViewModel.AssociationReport
{
    using System;

    public class AssociationDetailFilterViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string AccountStatusIds { get; set; }

        public int SiteId { get; set; }

        public string AssociationStatuses { get; set; }

        public int EvidenceId { get; set; }

        public int AssociationType { get; set; }

    }
}
