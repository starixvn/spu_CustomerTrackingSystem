namespace Fanex.NAP.CTS.Model.FraudCase.FlaggedAccount
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;
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