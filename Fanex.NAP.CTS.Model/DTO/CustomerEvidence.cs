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

        public DateTime CreatedDate
        {
            get
            {
                var converter = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return converter.AddMilliseconds(createdDateInTicks);
            }
        }

        public int AssociatedLevel { get; set; }
    }
}