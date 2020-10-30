namespace Fanex.NAP.CTS.Model.AssociationReport
{
    using System;

    public class AssociationModel
    {
        public int CTSCustId { get; set; }

        public int CustId { get; set; }

        public string UserName { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public string AccountStatus { get; set; }

        public string EvidenceCode { get; set; }

        public string CategoryNames { get; set; }
    }
}