namespace Fanex.NAP.CTS.Model.DTO
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using System;

    [XName("FlaggedCustomer")]
    public class CustomerEvidence
    {
        long createdDateInTicks;

        public int CustId { get; set; }

        public int Level0CustId { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }

        public long CreatedDateInTicks
        {
            set { createdDateInTicks = value; }
        }

        public DateTime CreatedDate { get; set; }

        public int AssociatedLevel { get; set; }
    }
}