namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using System;
    using System.Xml.Serialization;

    [XName("r")]
    public class LatestClassificationModel
    {
        [XName("id")]
        public int CustId { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }

        [XName("t")]
        public int Category { get; set; }

        [XmlIgnore]
        public DateTime ScannedTime { get; set; }
    }
}