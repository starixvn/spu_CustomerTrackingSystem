namespace Fanex.NAP.CTS.Library.Models.FraudCase.FlaggedAccount
{
    using Fanex.NAP.CTS.Library.Core.Helpers.Xml;
    using System.Xml.Serialization;

    [XName("CustomerEvidence")]
    public class FlaggedCustomer
    {
        public int CustId { get; set; }

        public string EvidenceCode { get; set; }

        [XmlIgnore]
        public string EvidenceName { get; set; }
    }
}