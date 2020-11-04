namespace Fanex.NAP.CTS.Model.AssociationReport
{
    using System;

    public class AssociationDetailModel
    {
        public int CTSCustId { get; set; }

        public string UserName { get; set; }

        public int SubscriberId { get; set; }

        public int? SiteID { get; set; }

        public string SiteName { get; set; }

        public string AccountStatus { get; set; }

        public DateTime AssociatedDate { get; set; }

        public string AssociatedDateString { get; set; }

        public int AssociationStatus { get; set; }

        public string AssociaionType { get; set; }
    }
}
