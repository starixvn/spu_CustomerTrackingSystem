namespace Fanex.NAP.CTS.ViewModel.AssociationReport
{
    using System;

    public class AssociationDetailViewModel
    {
        public int CTSCustId { get; set; }

        public string UserName { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public string AccountStatus { get; set; }

        public DateTime AssociatedDate { get; set; }

        public string AssociatedDateString { get; set; }

        public bool IsLinked { get; set; }

        public string AssociaionType { get; set; }

        public int TotalAssociationAccount { get; set; }
    }
}
