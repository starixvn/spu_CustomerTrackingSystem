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

        public string StatusName { get; set; }

        public DateTime AssociationDate { get; set; }

        public string AssociationDateString { get; set; }

        public int AssociationStatus { get; set; }

        public int AssociationType { get; set; }
    }
}
