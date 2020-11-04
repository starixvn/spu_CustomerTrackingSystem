namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System;
    using System.Collections.Generic;

    public class AssociationReportModel
    {
        public long CTSCustID { get; set; }

        public string Username { get; set; }

        public string AccountStatus { get; set; }

        public List<string> EvidenceCodes { get; set; }
    }
}