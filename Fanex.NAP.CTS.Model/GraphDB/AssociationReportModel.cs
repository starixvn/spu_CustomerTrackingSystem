namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System;

    public class AssociationReportModel
    {
        public long CTSCustID { get; set; }

        public string Username { get; set; }

        public string AccountStatus { get; set; } //todo: update after restructure grapdb

        public string EvidenceCode { get; set; }
    }
}