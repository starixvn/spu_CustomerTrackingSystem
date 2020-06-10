namespace Fanex.NAP.CTS.Model.DTO
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using System;

    [XName("FlaggedCustomer")]
    public class CustomerEvidence
    {
        public int CustId { get; set; }

        public int Level0CustId { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}