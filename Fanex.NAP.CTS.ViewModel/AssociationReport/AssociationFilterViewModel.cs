namespace Fanex.NAP.CTS.ViewModel.AssociationReport
{
    using System;

    public class AssociationFilterViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string AccountStatusIds { get; set; }

        public string SiteName { get; set; }

        public string AssociationStatuses { get; set; }

        public string EvidenceCode { get; set; }

        public int AssociationType { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

    }
}