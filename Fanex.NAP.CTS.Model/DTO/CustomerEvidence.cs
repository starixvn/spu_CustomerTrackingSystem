namespace Fanex.NAP.CTS.Model.DTO
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [XName("FlaggedCustomer")]
    public class CustomerEvidence
    {
        public int CustId { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }
    }
}